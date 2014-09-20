using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;

using Elco.Config;
using Elco.Services;
using Elco.Models;
using Controleng.Common;
using System.Text.RegularExpressions;

namespace Elco.Web.En.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            //设置模板页中变量
            ViewBag.RootCategoryInfo = CategoryService.Get(GeneralConfig.ProductRootId_DE);

            return View();
        }

        public ActionResult List() {
            //设置模板页中根类别
            //主要显示Banner图片
            ViewBag.RootCategoryInfo = CategoryService.Get(GeneralConfig.ProductRootId_DE);

            int catId = GeneralConfig.ProductRootId_EN;
            var absolutePath = Request.Url.AbsolutePath;
            Match m = Regex.Match(absolutePath,@"list\-(\d+)\.html",RegexOptions.IgnoreCase);
            if(m.Success){
                catId = Utils.StrToInt(m.Groups[1].Value, GeneralConfig.ProductRootId_EN);
            }
            var currentCategoryInfo = CategoryService.Get(catId);
            if(currentCategoryInfo.Id == 0 || currentCategoryInfo.IsDeleted){
                return View("Error");
            }

            //获得产品列表
            var productList = ProductService.List(new ProductSearchSetting() { 
                CategoryId = catId,
                PageIndex = CECRequest.GetQueryInt("page",1),
                PageSize = 10,
                ShowDeleted = false
            });

            ViewBag.List = productList;

            //
            ViewBag.CurrentCategoryInfo = currentCategoryInfo;


            return View();
        }

        #region == 输出左边导航的下拉列表 == 
        /// <summary>
        /// 输出左边导航的下拉列表
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderProductDropDownList(object selectedValue) {
            StringBuilder sbHtml = new StringBuilder();

            var oldCatList = CategoryService.ListAllSubCatById(GeneralConfig.ProductRootId_DE).Where(p => p.IsEnabled == true
                && p.IsDeleted == false);
            var newCatList = new List<CategoryInfo>();

            Action<IEnumerable<CategoryInfo>,List<CategoryInfo>,int,int> fb = null;
            fb = (oldList,newList,parentId,depath) => {
                var tempList = oldList.Where(p => p.ParentId == parentId).OrderBy(p => p.Sort);
                foreach (var item in tempList)
                {
                    item.Name = string.Format("{0}{1}", new string('#', depath), item.Name);
                    newList.Add(item);
                    var ishasChild = oldList.Where(p => p.ParentId == item.Id).Count() > 0;
                    if (ishasChild)
                    {
                        fb(oldList,newList,item.Id,(depath + 1));
                    }
                }
            };
            fb(oldCatList,newCatList, 0, 0);

            sbHtml.Append("<select name=\"ddlProductCat\" id=\"ddlProductCat\" class=\"prodSelect\"  onchange=\"window.location.href=this.value;\">");
            foreach(var item in newCatList){
                sbHtml.AppendFormat("<option value=\"/product/{1}\" parentid=\"{2}\" {3}>{0}</option>", 
                    item.Name.Replace("#", "&nbsp;&nbsp;"), 
                    //item.Name,
                    item.Id == GeneralConfig.ProductRootId ? string.Empty : string.Format("list-{0}.html",item.Id),
                    item.ParentId,
                    item.Id.Equals(selectedValue) ? "selected=\"selected\"" : string.Empty
                );
            }
            sbHtml.Append("</select>");

            return Content(sbHtml.ToString());
        }
        #endregion

        #region == 输出左边菜单 ==
        /// <summary>
        /// 输出左边菜单
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderProductMenu() {
            StringBuilder sbHtml = new StringBuilder();
            var allCatList = CategoryService.ListAllSubCatById(GeneralConfig.ProductRootId_DE).Where(p => p.IsDeleted == false && p.IsEnabled == true);

            Action<IEnumerable<CategoryInfo>, int,int, StringBuilder> fb = null;
            fb = (catList, parentId,level,sb) => {
                var tempList = catList.Where(p => p.ParentId == parentId ).OrderBy(p => p.Sort);

                //判断是否是UL容器
                //如果是，则添加根class，否则添加ul_level_0
                string ulLevalClass = string.Format("ul-leval-{0}", level);
                string className = parentId == GeneralConfig.ProductRootId_DE ? "class=\"product-menu\"" : string.Empty;

                if (parentId != GeneralConfig.ProductRootId_DE && tempList.Count() > 0)
                {
                    sb.Append("<!--[if lte IE 6]><table><tr><td><![endif]-->");
                }
                sb.AppendFormat("<ul {1}{0}>", string.Empty, parentId == GeneralConfig.ProductRootId_DE ? "id=\"product_menu\"" : string.Format("id=\"p_menu_ul_{0}\"", parentId));
                

                foreach (var item in tempList)
                {
                    //判断是否子分类
                    var ishasChild = catList.Where(p => p.ParentId == item.Id).Count() > 0;

                    string liLevelClass = string.Format(" class=\"li-level-{0}\"",level);                  

                    sb.AppendFormat("<li {0}>",string.Empty);
                    sb.AppendFormat("<a href=\"/product/list-{1}.html\" id=\"p_menu_{1}\" title=\"{0}\" class=\"{2}\">{0}", item.Name, item.Id, ishasChild ? "sub-icon" : string.Empty);
                    if(ishasChild){
                        sb.Append("<!--[if IE 7]><!-->");
                    }
                    sb.Append("</a>");
                    if (ishasChild)
                    {
                        sb.Append("<!--<![endif]-->");
                    }
                    
                    if (ishasChild)
                    {
                        fb(catList,item.Id,level+1,sb);
                    }
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
                if (parentId != GeneralConfig.ProductRootId_DE && tempList.Count() > 0)
                {
                    sb.Append("<!--[if lte IE 6]></td></tr></table></a><![endif]-->");
                }
            };
            fb(allCatList, GeneralConfig.ProductRootId_DE, 0, sbHtml);

            return Content(sbHtml.ToString());
        }
        #endregion
    }
}
