using System.Web.Mvc;

namespace Elco.Web.En.Areas.PagesAdmin
{
    public class PagesAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PagesAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PagesAdmin_login",
                "PagesAdmin/login",
                new { action = "Login", controller = "Home", id = UrlParameter.Optional },
                new string[] { "Elco.Web.En.Areas.PagesAdmin.Controllers" }
            );
            context.MapRoute(
                "PagesAdmin_logout",
                "PagesAdmin/logout",
                new { action = "Logout", controller = "Home", id = UrlParameter.Optional },
                new string[] { "Elco.Web.En.Areas.PagesAdmin.Controllers" }
            );
            context.MapRoute(
                "PagesAdmin_default",
                "PagesAdmin/{controller}/{action}/{id}",
                new { action = "Index", controller="Home", id = UrlParameter.Optional },
                new string[] { "Elco.Web.En.Areas.PagesAdmin.Controllers" }
            );
        }
    }
}
