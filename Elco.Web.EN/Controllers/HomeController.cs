using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Elco.Services;
using Elco.Models;
using System;
using System.Collections.Generic;
using Elco.Config;


namespace Elco.Web.En.Controllers
{
    public class HomeController : BaseController
    {
        
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            //焦点图片
            var focus = ArticleService.ListWithoutPage(91,4);
            //焦点图有两张图（大图和小图）
            //大图用ImageUrl,小图用Content中第一个图片
            foreach(var item in focus){
                //处理一下Content
                MatchCollection mc = Regex.Matches(item.Content, @"src[\s]*=[\s]*[""']?[\s]*([^""'>]*)[^>]*?");
                string firstImageUrl = string.Empty;
                foreach(Match m in mc){
                    if(m.Success){
                        firstImageUrl = m.Groups[1].Value;
                    }
                    if(!string.IsNullOrEmpty(firstImageUrl)){
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(firstImageUrl))
                {
                    item.Content = firstImageUrl;
                }
                else {
                    item.Content = item.ImageUrl;
                }
            }


            ViewBag.Focus = focus;


            return View("NewIndex");
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Test() {            
            return View();
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ServerInfo() {
            return View();
        }

        #region == 输出左边菜单 ==
        /// <summary>
        /// 输出左边菜单
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderLeftCategoryMenu(CategoryInfo currentCategoryInfo, CategoryInfo rootCategoryInfo,string linkFormat)
        {          

            int rootId = rootCategoryInfo.Id;
            StringBuilder sbHtml = new StringBuilder();
            var allCatList = CategoryService.ListAllSubCatById(rootId).Where(p => p.IsDeleted == false && p.IsEnabled == true);

            Action<IEnumerable<CategoryInfo>, int, int, StringBuilder> fb = null;
            fb = (catList, parentId, level, sb) =>
            {
                var tempList = catList.Where(p => p.ParentId == parentId).OrderBy(p => p.Sort);

                string className = parentId == rootId ? "category-menu" : "none";
                sb.AppendFormat("<ul class=\"{0}\" {1}>", className, parentId == rootId ? "id=\"category_menu\"" : string.Empty);

                if (tempList.Count() == 0)
                {
                    //如果没有分类，则显示父分类名称
                    sbHtml.AppendFormat("<li class=\"on\"><a href=\"{1}\">{0}</a></li>", rootCategoryInfo.Name, Request.Url.AbsolutePath);
                }
                else
                {

                    foreach (var item in tempList)
                    {
                        //判断是否子分类
                        var ishasChild = catList.Where(p => p.ParentId == item.Id).Count() > 0;

                        if (currentCategoryInfo != null && currentCategoryInfo.Id == item.Id)
                        {
                            sb.Append("<li class=\"on\">");
                        }
                        else
                        {
                            sb.Append("<li>");
                        }
                        sb.AppendFormat("<a href=\"{3}\" id=\"d_menu_{1}\" title=\"{0}\" class=\"{2}\">{0}</a>", item.Name, item.Id, ishasChild ? "sub-icon" : string.Empty,string.IsNullOrEmpty(linkFormat) ? item.Url : string.Format(linkFormat,item.Id));


                        if (ishasChild)
                        {
                            fb(catList, item.Id, level + 1, sb);
                        }
                        sb.Append("</li>");
                    }
                }
                sb.Append("</ul>");
            };
            fb(allCatList, rootId, 0, sbHtml);

            return Content(sbHtml.ToString());
        }
        #endregion

        #region == 输出右边导航 ChildActionOnly ==
        /// <summary>
        /// 输出右边导航
        /// </summary>
        /// <param name="currentCategoryInfo"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RenderNav(CategoryInfo currentCategoryInfo, string linkFormat, CategoryInfo rootCategoryInfo, bool isShowTitle = false)
        {
            /*
             <p class="floatR"><strong>当前位置： </strong><a href="/">首页</a> &gt; @(currentCategoryInfo.Name)</p>      <h3>@(currentCategoryInfo.Name)</h3>
             */

            linkFormat = string.IsNullOrEmpty(linkFormat) ? string.Empty : linkFormat;

            StringBuilder sb = new StringBuilder();
            sb.Append("<p class=\"floatR\"><strong>Current Location： </strong><a href=\"/\">Home</a>");

            //递归输出导航
            var parentCatList = CategoryService.ListUpById(currentCategoryInfo.Id);
            if (parentCatList.Count == 0)
            {
                //说明是产品中心首页
                sb.AppendFormat("&nbsp;&gt;&nbsp;{0}", currentCategoryInfo.Name);
            }
            foreach (var item in parentCatList)
            {
                if (/*item.IsEnabled &&*/ !item.IsDeleted)
                {
                    sb.AppendFormat("&nbsp;&gt;&nbsp;<a href=\"{1}\" id=\"nav_{2}\" title=\"{0}\">{0}</a>", item.Name, string.IsNullOrEmpty(item.LinkUrl) ? string.Format(linkFormat, item.Id) : item.LinkUrl, item.Id);
                }
            }
            sb.Append("</p>");
            //输出当前名称
            sb.AppendFormat("<h3>{0}</h3>", isShowTitle ? currentCategoryInfo.Name : "&nbsp;");
            return Content(sb.ToString());
        }
        #endregion

        #region == 查询 ==
        public ActionResult Search()
        {

            /*
             * 设置_CommmonLayout.cshtml中的变量
             */

            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = CategoryService.Get(GeneralConfig.NewsRootId);

            return View();
        }
        #endregion
    }
}
