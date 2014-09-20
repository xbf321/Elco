using System.Web.Mvc;
using System.Web.Routing;
using Elco.Mvc.Filters;

namespace Elco.Web.WWW
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
		    //错误拦截器
            filters.Add(new SilenceHandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            #region == News ==
            routes.MapRoute(
                "ArticleShow",
                "n/{fulltimespan}.html",
                new { controller = "News", action = "Show", fulltimespan = @"\d+" },
                new string[] { "Elco.Web.WWW.Controllers" }
            );
            #endregion

            #region == product/list-xx.html ==
            routes.MapRoute(
                "ProductList",
                "product/list-{catId}.html",
                new { controller = "Product", action = "List", catId = @"\d+" },
                new string[] { "Elco.Web.WWW.Controllers" }
            );
            #endregion

            #region == Channel ==
            routes.MapRoute(
                "Channel_Show", // Route name
                "channel/{value}.html", // URL with parameters
                new { controller = "Channel", action = "Show"}, // Parameter defaults
                new string[] { "Elco.Web.WWW.Controllers" }
            );
            #endregion

            #region == Search ==
            routes.MapRoute(
                "Search", // Route name
                "Search", // URL with parameters
                new { controller = "Home", action = "Search" }, // Parameter defaults
                new string[] { "Elco.Web.WWW.Controllers" }
            );
            #endregion

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new string[] { "Elco.Web.WWW.Controllers" }
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}