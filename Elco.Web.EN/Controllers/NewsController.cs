using System.Text;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Elco.Models;
using Elco.Services;
using Elco.Config;
using Controleng.Common;


namespace Elco.Web.En.Controllers
{
    public class NewsController : BaseController
    {

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
                return Content("This article does not exist or has been deleted.<a href=\"/\">go back</a>");
            }

            //获取此文章所属类别ID，用于左边导航列表
            var currentCategoryInfo = CategoryService.Get(articleInfo.CategoryId);
            if (currentCategoryInfo.IsEnabled == false && currentCategoryInfo.IsDeleted == true)
            {
                //说明此分类设置为未启用或者分类已删除
                return Content("Classification this article has been deleted or set to not enabled！");
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
