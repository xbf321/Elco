using System;
using System.Web.Mvc;

namespace Elco.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class PagesAdminAuthAttribute : FilterAttribute, IActionFilter
    {
        #region IActionFilter Members

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //判断登录
            if(!ElcoHttpContext.Current.IsAdminLogin){
                filterContext.HttpContext.Response.Redirect("/pagesadmin/login");
            }

        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        

        #endregion
    }
}
