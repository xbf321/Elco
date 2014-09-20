using System.Web.Mvc;

using Elco.Services;
using Elco.Config;
using Elco.Models;

namespace Elco.Web.WWW.Controllers
{
    /// <summary>
    /// 投诉与建议
    /// </summary>
    public class FeedbackController : BaseController
    {
       
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            /*
             * 设置_CommmonLayout.cshtml中的变量
             */

            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = CategoryService.Get(GeneralConfig.FeedbackRootId);

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FeedbackInfo model,FormCollection fc) {
            
            /*
             * 设置_CommmonLayout.cshtml中的变量
             */

            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = CategoryService.Get(GeneralConfig.FeedbackRootId);

            bool errors = false;
            if(string.IsNullOrEmpty(model.Title)){
                errors = true;
                ModelState.AddModelError("Title", "请输入投诉(建议)标题");
            }
            if(string.IsNullOrEmpty(model.Content)){
                errors = true;
                ModelState.AddModelError("Content", "请输入投诉(建议)内容");
            }

            if(ModelState.IsValid && !errors){

                //过滤掉内容HTML
                model.Content = Goodspeed.Library.Char.HtmlHelper.RemoveHtml(model.Content);

                int id = FeedbackService.Create(model).Id;
                ViewBag.Status = false;
                if(id>0){
                    ViewBag.Status = true;
                }
            }

            return View();
        }

    }
}
