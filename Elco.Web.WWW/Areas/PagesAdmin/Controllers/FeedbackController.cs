using System.Web.Mvc;

using Elco.Services;
using Elco.Models;
using Controleng.Common;
using Elco.Mvc.Filters;

namespace Elco.Web.WWW.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class FeedbackController : Controller
    {
        //
        // GET: /PagesAdmin/Feedback/

        public ActionResult List()
        {
            var list = FeedbackService.List(new SearchSetting() { 
                PageIndex = CECRequest.GetQueryInt("page",1),
                PageSize = 20
            });

            ViewBag.List = list;

            return View();
        }

    }
}
