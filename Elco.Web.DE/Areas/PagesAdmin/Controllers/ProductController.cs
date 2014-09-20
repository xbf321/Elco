using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

using Elco.Services;
using Elco.Models;
using Controleng.Common;
using Elco.Config;
using Elco.Mvc.Filters;

namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    internal class ShortCategoryInfo
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [PagesAdminAuth]
    public class ProductController : Controller
    {
        #region == 产品分类 ==
        /// <summary>
        /// 产品分类列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CatList() {

            //获得产品中心下的所有分类
            var productCategoryList = CategoryService.ListAllSubCatById(Elco.Config.GeneralConfig.ProductRootId_EN);   


            var catHtml = BuildListForAdminWithEdit(productCategoryList, 0);
            ViewBag.CatHtml = catHtml;
            return View();
        }
        private string BuildListForAdminWithEdit(IEnumerable<CategoryInfo> list, int parentId)
        {
            var pList = list.Where(nc => nc.ParentId == parentId);
            if (pList.Count() == 0) { return string.Empty; }
            var sb = new StringBuilder();
            sb.AppendFormat("<ul {0}>", parentId == 0 ? "id=\"treeview\"" : string.Empty);
            foreach (var item in pList)
            {
                if (item.IsDeleted) { continue; }
                sb.Append("<li>");
                sb.AppendFormat("<span class=\"cat\"><span class=\"cat-name\">{0}（{1}）</span>", item.Name, item.Id);

                sb.Append("<span class=\"cat-operate\">");
 
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"catcreate?catId={0}\">添加子分类</a>", item.Id);
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"catcreate?id={0}\">编辑</a>", item.Id);
                if (!item.IsEnabled)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">未启用</font>");
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'enable')\">启用</a>", item.Id);
                }
                else
                {
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'disable')\">禁用</a>", item.Id);
                }
                if (item.IsDeleted)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">已删除</font>");
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'restore')\">还原</a>", item.Id);
                }
                else
                {
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'delete')\">删除</a>", item.Id);
                }
                sb.Append("</span></span>");

                //递归
                sb.Append(BuildListForAdminWithEdit(list, item.Id));
                sb.AppendLine("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        
        #endregion

        #region == 添加和编辑产品分类 ==
        public ActionResult CatCreate()
        {
            int catId = CECRequest.GetQueryInt("id", 0);
            var model = CategoryService.Get(catId);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CatCreate(CategoryInfo modelInfo, FormCollection fc)
        {
            //判断是否选择中文
            //如果选择中文，则是根目录，否则为子类别
            bool isAdd = modelInfo.Id == 0 ? true : false;
            bool errors = false;
            int catId = Controleng.Common.Utils.StrToInt(fc["ddlCat"], 0);
            if (modelInfo.Id != GeneralConfig.ProductRootId_EN)
            {
                if (catId == 0)
                {
                    errors = true;
                    ModelState.AddModelError("CAT", "请选择分类");
                }
            }
            if (string.IsNullOrEmpty(modelInfo.Name))
            {
                errors = true;
                ModelState.AddModelError("NAME", "请输入名称");
            }

            var parentModelInfo = CategoryService.Get(catId);
            if (CategoryService.ExistsName(modelInfo.Id, modelInfo.Name, parentModelInfo.Id))
            {
                errors = true;
                ModelState.AddModelError("NAME", "分类名称不能重复");
            }
            if (!errors && ModelState.IsValid)
            {

                //在这没判断别名是否重复，暂时没做

                modelInfo.ParentId = catId;
                if (catId == 0)
                {
                    modelInfo.ParentIdList = "0";
                }
                else
                {
                    modelInfo.ParentIdList = string.Format("{0},{1}", parentModelInfo.ParentIdList, modelInfo.ParentId);
                }

                CategoryService.Create(modelInfo);
                if (isAdd)
                {
                    ViewBag.Msg = string.Format("添加成功！是否继续添加？<a href=\"catcreate?catId={0}\">【是】</a>&nbsp;&nbsp;<a href=\"catlist\">【否】</a>", CECRequest.GetQueryInt("catId", 0));
                }
                else
                {
                    ViewBag.Msg = "更新成功！<a href=\"catlist\">返回</a>";
                }
            }

            return View(modelInfo);
        }
        #endregion

        #region == 添加 或 编辑 ==
        public ActionResult Create()
        {
            int id = CECRequest.GetQueryInt("id",0);
            var model = ProductService.Get(id);

            

            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ProductInfo model,FormCollection fc) {

            bool errors = false;
            bool isAdd = model.Id == 0 ? true : false;
            if(string.IsNullOrEmpty(model.Title)){
                errors = true;
                ModelState.AddModelError("Title","产品名称不能为空");
            }
            if(!errors && ModelState.IsValid){

                //添加产品属性
                foreach (var item in fc.AllKeys)
                {
                    Match m = Regex.Match(item, @"prop_value_(\d+)", RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        int propId = Utils.StrToInt(m.Groups[1].Value, 0);
                        if (propId > 0 && !string.IsNullOrEmpty(fc[item]))
                        {
                            model.Props.Add(new ProductPropInfo()
                            {
                                Id = propId,
                                Value = fc[item]
                            });
                        }
                    }
                }

                //保存
                model = ProductService.Create(model);

                

                if (isAdd)
                {
                    ViewBag.Msg = "添加成功！是否继续？【<a href=\"create\">是</a>】&nbsp;【<a href=\"list\">否</a>】";
                }
                else
                {
                    ViewBag.Msg = "修改成功！是否继续？【<a href=\"create\">是</a>】&nbsp;【<a href=\"list\">否</a>】";
                }
            }
            return View(model);
        }
        #endregion

        #region == 列表 ==
        public ActionResult List() {


            

            int pageIndex = CECRequest.GetQueryInt("page",1);
            int catId = CECRequest.GetQueryInt("catId",0);


            var oldCatList = CategoryService.ListAllSubCatById(GeneralConfig.ProductRootId_EN);
            var newCatList = new List<CategoryInfo>();

            Action<IEnumerable<CategoryInfo>, List<CategoryInfo>, int, int> fb = null;
            fb = (oldList, newList, parentId, depath) =>
            {
                var tempList = oldList.Where(p => p.ParentId == parentId);
                foreach (var item in tempList)
                {
                    item.Name = string.Format("{0}├{1}", new string('│', depath), item.Name);
                    newList.Add(item);
                    var ishasChild = oldList.Where(p => p.ParentId == item.Id).Count() > 0;
                    if (ishasChild)
                    {
                        fb(oldList, newList, item.Id, (depath + 1));
                    }
                }
            };
            fb(oldCatList, newCatList, 0, 0);

            ViewBag.CatList = newCatList;

            var list = ProductService.List(new ProductSearchSetting() { 
                PageIndex = pageIndex,
                PageSize = 20,
                CategoryId = catId,
                ShowDeleted = true
            });

            ViewBag.List = list;

            return View();
        }
        #endregion

        #region == 产品属性管理 ==
        public ActionResult Prop()
        {

            //获取产品中心一级分类
            var parentCatList = CategoryService.ListByParentId(Elco.Config.GeneralConfig.ProductRootId_EN);
            ViewBag.ParentCatList = parentCatList;


            return View();
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Prop(FormCollection fc)
        {
            int leafCid = Utils.StrToInt(fc["leafCid"], 0);
            int propId = Utils.StrToInt(fc["propId"], 0);
            bool error = true;
            string msg = string.Empty;
            string action  = fc["action"] ?? string.Empty;
            try
            {
                if (action.Equals("deleteprop", StringComparison.OrdinalIgnoreCase))
                {
                    //删除
                    int i = ProductPropService.Delete(propId,leafCid);
                    if(i>0){
                        error = false;
                    }
                }
                else if (action.Equals("addbatchprops",StringComparison.OrdinalIgnoreCase)) { 
                    //批量保存属性名
                    string propsName = fc["name"] ?? string.Empty;
                    propsName = propsName.Replace("\r", string.Empty).Replace("\n","|");
                    string[] propsNameArr = propsName.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
                    foreach(var name in propsNameArr){
                        ProductPropService.Create(new ProductPropInfo() { 
                            CategoryId = leafCid,
                            Name = name
                        });
                    }
                    error = false;
                }
                else
                {
                    //添加或编辑
                    string propName = fc["name"] ?? string.Empty;

                    var model = ProductPropService.Create(new ProductPropInfo()
                    {
                        CategoryId = leafCid,
                        Id = propId,
                        Name = propName
                    });
                    if (model.Id > 0)
                    {
                        error = false;
                    }

                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return Json(new { error = error, msg = msg });
        }
        /// <summary>
        /// 异步回调地址
        /// </summary>
        /// <returns></returns>
        public ActionResult Ajax()
        {
            string action = CECRequest.GetQueryString("action");
            if (action.Equals("childcid", StringComparison.OrdinalIgnoreCase))
            {
                //获得子分类
                IList<ShortCategoryInfo> list = new List<ShortCategoryInfo>();
                int cid = CECRequest.GetQueryInt("cid", 0);
                var tempList = CategoryService.ListByParentId(cid);
                foreach (var item in tempList)
                {
                    list.Add(new ShortCategoryInfo()
                    {
                        ParentId = item.ParentId,
                        Id = item.Id,
                        Name = item.Name.Replace('"','”')
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            if (action.Equals("props", StringComparison.OrdinalIgnoreCase))
            {
                //获得属性列表
                int cid = CECRequest.GetQueryInt("cid", 0);
                List<ProductPropInfo> list = new List<ProductPropInfo>();
                list = ProductPropService.List(cid);
                foreach(var item in list){
                    item.Name = item.Name.Replace('"','”');
                    item.Value = item.Value.Replace('"', '”');
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(new { });
        }
        #endregion

        #region == 输出产品类别JSON数据 ChildActionOnly ==
        /// <summary>
        /// 输出产品类别JSON数据
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderCatJSON() {
            StringBuilder json = new StringBuilder();
            var list = CategoryService.ListAllSubCatById(Elco.Config.GeneralConfig.ProductRootId_EN).Where(p => { return p.IsDeleted == false && p.ParentId != 0; });
            int i = 0, l = list.Count();
            json.Append("[");
            foreach(var item in list){
                json.Append("{");
                json.AppendFormat("id:{0},pid:{1},name:\"{2}\"", item.Id, item.ParentId, item.Name.Replace('"','”'));
                json.Append("}");
                if (i != l - 1)
                {
                    json.Append(",");
                }
                i++;                
            }
            json.Append("]");
            return Content(json.ToString());
        }
        #endregion
    }
}
