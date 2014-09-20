using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

using Elco.Models;
using Elco.Services;
using Controleng.Common;

namespace Elco.Web.WWW.Controllers
{
    public class ChannelController : BaseController
    {
        public ActionResult Show(string value)
        {
            //Channel格式，必须为/Channel/(\d+).html
            //优先选择这样的格式
            int categoryId = 0;
            string fileName = Goodspeed.Web.UrlHelper.Current.FileName;
            string _urlCatName = Regex.Match(fileName, @"(\w+)\.html", RegexOptions.IgnoreCase).Groups[1].Value;
            if (Regex.IsMatch(_urlCatName, @"\d+"))
            {
                //不是别名
                categoryId = Controleng.Common.Utils.StrToInt(_urlCatName, 0);
            }
            else
            {
                //是别名的情况
                categoryId = CategoryService.ListAll().FirstOrDefault(p => p.Alias == _urlCatName).Id;
            }
            //当前节点
            var currentCategoryInfo = CategoryService.Get(categoryId);
            if (currentCategoryInfo.Id>0 && currentCategoryInfo.IsDeleted == false)
            {
                LoadExtensionInfo(currentCategoryInfo);
            }

            

            return View();
            
        }
        [NonAction]
        private void LoadExtensionInfo(CategoryInfo currentCatInfo) {
            CategoryInfo rootCatInfo = currentCatInfo;

            //获取当前节点的顶级父节点
            //可以是未启用的，因为有的列表就是页面顶部导航不显示，但是左边需要显示
            //不显示已删除的
            var _rootCatInfo = CategoryService.ListUpById(currentCatInfo.Id).Where(p => p.IsDeleted == false).FirstOrDefault();
            if (_rootCatInfo != null)
            {
                rootCatInfo = _rootCatInfo;
            }
            //是否显示子分类的首节点
            //如果是，当前节点更改为子节点
            //只显示启用的以及未删除的
            if (currentCatInfo.IsShowFirstChildNode)
            {
                var firstChildNode = CategoryService.ListByParentId(currentCatInfo.Id).Where(p => (p.IsDeleted == false && p.IsEnabled == true)).FirstOrDefault();
                if (firstChildNode != null)
                {
                    currentCatInfo = firstChildNode;
                }
            }
            ViewBag.RootCategoryInfo = rootCatInfo;
            ViewBag.CurrentCategoryInfo = currentCatInfo;

            //模板类型
            switch (currentCatInfo.TemplateType)
            {
                case (int)TemplateType.ArticleList:
                case (int)TemplateType.ArticleListWithImage:
                    int pageIndex = CECRequest.GetQueryInt("page", 1);
                    ViewBag.ArticleList = ArticleService.List(new ArticleSearchSetting
                    {
                        PageIndex = pageIndex,
                        CategoryId = currentCatInfo.Id,
                        IsOnlyShowPublished = true
                    });
                    break;
            }
            
        }

    }
}
