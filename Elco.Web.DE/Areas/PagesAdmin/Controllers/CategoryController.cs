using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web.Mvc;

using Controleng.Common;
using Elco.Services;
using Elco.Models;
using Elco.Mvc.Filters;
using System;
using Elco.Common;
using System.Data;

namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class CategoryController : Controller
    {
        #region == List ==
        public ActionResult List()
        {
            var all = CategoryService.ListAll(); //后台显示所有的，包含删除的和未启用的
            //获得产品中心下的所有分类
            var productCategoryList = CategoryService.ListAllSubCatById(Elco.Config.GeneralConfig.ProductRootId_DE);

            //排除掉产品中心所有的分类
            var list = all.Except(productCategoryList);

            var catHtml = BuildListForAdminWithEdit(list, 0);
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
                sb.Append("<span>");
                sb.AppendFormat("<span class=\"cat-name\">{0}（{1}）</span>", item.Name, item.Id);
                sb.Append("<span class=\"cat-operate\">");
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"create?catId={0}\">Add sub-categories</a>", item.Id);
                sb.AppendFormat("&nbsp;&nbsp;<a href=\"create?id={0}\">Edit</a>", item.Id);
                if (!item.IsEnabled)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">Not Enabled</font>");
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'enable')\">Enable</a>", item.Id);
                }
                else
                {
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'disable')\">Disable</a>", item.Id);
                }
                if (item.IsDeleted)
                {
                    sb.Append("&nbsp;&nbsp;<font color=\"red\">Deleted</font>");
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'restore')\">Restore</a>", item.Id);
                }
                else
                {
                    sb.AppendFormat("&nbsp;&nbsp;<a href=\"###\" onclick=\"operate({0},'delete')\">Delete</a>", item.Id);
                }
                sb.Append("</span>");
                sb.Append("</span>");
                //递归
                sb.Append(BuildListForAdminWithEdit(list, item.Id));
                sb.AppendLine("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }
        #endregion

        #region == Create OR Edit ==

        public ActionResult Create()
        {
            int catId = CECRequest.GetQueryInt("id", 0);
            var model = CategoryService.Get(catId);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection fc, CategoryInfo modelInfo)
        {
            bool isAdd = modelInfo.Id == 0 ? true : false;
            bool errors = false;
            int catId = Controleng.Common.Utils.StrToInt(fc["ddlCat"], -1);
            if (string.IsNullOrEmpty(modelInfo.Name))
            {
                errors = true;
                ModelState.AddModelError("NAME", "Please enter a name");
            }

            var parentModelInfo = CategoryService.Get(catId);
            if (CategoryService.ExistsAlias(modelInfo.Id, modelInfo.Alias))
            {
                ModelState.AddModelError("ALIAS", "Alias ​​already exists, please choose another alias");
            }
            /*if (CategoryService.ExistsName(modelInfo.Id, modelInfo.Name, parentModelInfo.Id))
            {
                errors = true;
                ModelState.AddModelError("NAME", "分类名称不能重复");
            }*/
            if (!errors && ModelState.IsValid)
            {

                //在这没判断别名是否重复，暂时没做


                modelInfo.ParentId = catId;
                if (catId == 0)
                {
                    //说明是选中的语言
                    modelInfo.ParentId = 0;
                    modelInfo.ParentIdList = "0";
                }
                else
                {
                    modelInfo.ParentIdList = string.Format("{0},{1}", parentModelInfo.ParentIdList, modelInfo.ParentId);
                }

                CategoryService.Create(modelInfo);
                if (isAdd)
                {
                    ViewBag.Msg = string.Format("Add success.Continue？<a href=\"Create?catId={0}\">【Yes】</a>&nbsp;&nbsp;<a href=\"List\">【No】</a>", catId);
                }
                else
                {
                    ViewBag.Msg = ("Update success.<a href=\"List\">Back</a>");
                }
            }

            return View(modelInfo);
        }
        #endregion

        #region == Operate ==
        public ActionResult Operate()
        {
            int id = CECRequest.GetQueryInt("id", 0);
            string action = CECRequest.GetQueryString("action");
            if (action.Equals("delete", System.StringComparison.OrdinalIgnoreCase))
            {
                //删除
                CategoryService.Delete(id);
            }
            if (action.Equals("restore", System.StringComparison.OrdinalIgnoreCase))
            {
                CategoryService.Restore(id);
            }
            if (action.Equals("enable", System.StringComparison.OrdinalIgnoreCase) || action.Equals("disable", System.StringComparison.OrdinalIgnoreCase))
            {
                CategoryService.Enable(id);
            }
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return Redirect("List");
        }
        #endregion

        #region == 输出分类下拉列表 ChildActionOnly ==
        [ChildActionOnly]
        public ActionResult RenderCategoryDropdown(string name, object selectedValue)
        {
            //输出下载所属分类
            //输出下拉列表(排除掉产品分类)
            var all = Elco.Services.CategoryService.ListAll().Where(p => p.IsDeleted == false);
            var exceptCatList = all.Except(Elco.Services.CategoryService.ListAllSubCatById(new int[]{Elco.Config.GeneralConfig.ProductRootId_DE}));

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<select id=\"{0}\" name=\"{0}\">", name);
            sbHtml.Append("<option value=\"0\">==Root==</option>");

            //输出中文或英文类别
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
            fb(exceptCatList, newCatList, 0, 0);


            foreach (var item in newCatList)
            {
                sbHtml.AppendFormat("<option value=\"{1}\" {2}>{0}</option>", item.Name, item.Id, selectedValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty);
            }




            sbHtml.Append("</select>");
            return Content(sbHtml.ToString());


        }
        #endregion
    }
}
