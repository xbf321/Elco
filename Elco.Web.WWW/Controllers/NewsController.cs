using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Elco.Models;
using Elco.Services;
using Elco.Config;
using Controleng.Common;


namespace Elco.Web.WWW.Controllers
{
    public class NewsController : BaseController
    {
        /// <summary>
        /// 新闻中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            /*
             * 设置_CommmonLayout.cshtml中的变量
             */

            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = CategoryService.Get(GeneralConfig.NewsRootId);

            //焦点图片
            var focus = ArticleService.ListWithoutPage(GeneralConfig.NewsRootId, 4);
            ViewBag.Focus = focus;


            int pageIndex = CECRequest.GetQueryInt("page",1);
            var allNewsList = ArticleService.List(new ArticleSearchSetting() { 
                PageIndex = pageIndex,
                CategoryId = GeneralConfig.NewsRootId,
                IsOnlyShowPublished = true,
                Title = CECRequest.GetQueryString("key")
            });

            ViewBag.ArticleList = allNewsList;

            return View();
        }
        /// <summary>
        /// 获得左边区域最新动态新闻
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult _LatestNewsControll() {

            //新闻”2条，“精品”2条，“应用”2条 “系统集成”1条

            //新闻
            var news = ArticleService.ListWithoutPage(GeneralConfig.NewsRootId, 2);
            //精品
            var jingpin = ArticleService.ListWithoutPage(3,2);
            var application = ArticleService.ListWithoutPage(5,2);
            //系统
            var system = ArticleService.ListWithoutPage(7,1);

            var list = news.Concat(jingpin).Concat(application).Concat(system);
         
            StringBuilder sb = new StringBuilder();
            foreach(var item in list){
                sb.AppendFormat("<li><a href=\"{1}\" title=\"{0}\">{0}</a></li>",Goodspeed.Common.CharHelper.Truncate(item.Title,14),item.Url);
            }
            return Content(sb.ToString());
        }

        /// <summary>
        /// 文章显示
        /// </summary>
        /// <returns></returns>
        public ActionResult Show()
        {
            /*
             总模板页需要以下变量
             * 1，根据Url获得根节点信息，因为左边需要导航信息
             * 2，根据Url获得所属类别，因为右边区域有个副导航
             */
            string fileName = Goodspeed.Web.UrlHelper.Current.FileName;
            string fullTimespan = string.Empty;
            Match m = Regex.Match(fileName, @"(\d+)\.html", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                fullTimespan = m.Groups[1].Value ;
                
            }
            var articleInfo = ArticleService.GetByFullTimespan(fullTimespan);

            if (articleInfo.Id == 0)
            {
                return Content("此文章不存在或已删除，请<a href=\"/\">返回</a>");
            }

            //获取此文章所属类别ID，用于左边导航列表
            var currentCategoryInfo = CategoryService.Get(articleInfo.CategoryId);
            if (currentCategoryInfo.IsEnabled == false && currentCategoryInfo.IsDeleted == true)
            {
                //说明此分类设置为未启用或者分类已删除
                return Content("此文章的分类已删除或设置为未启用！");
            }

            var s = Request.Url;

            //获得根类别，用于左边导航列表
            //在这里顶级类别可以是已删除或未启用的状态
            //具体判断在显示左边导航列表的时候执行
            var rootCategoryInfo = CategoryService.ListUpById(currentCategoryInfo.Id).FirstOrDefault();
            ViewBag.RootCategoryInfo = rootCategoryInfo;
            ViewBag.CurrentCategoryInfo = currentCategoryInfo;

            //更新ViewCount
            ArticleService.UpdateViewCount(articleInfo.Id);

            return View(articleInfo);
        }

    }
}
