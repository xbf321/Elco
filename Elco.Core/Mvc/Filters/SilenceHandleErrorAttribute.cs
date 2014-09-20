using System;
using System.Web.Mvc;
using Common.Logging;

namespace Elco.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SilenceHandleErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string controller = filterContext.RouteData.Values["controller"] as string;
            string action = filterContext.RouteData.Values["action"] as string;

            //记录日志
            string msg = string.Format("Msg:Controller:{0},Action:{1}发生异常!\r\nUrl:{2}\r\nUserAgent:{3}\r\nReferrer:{4}\r\nIP:{5}\n",
                controller,
                action,
                filterContext.HttpContext.Request.Url,
                filterContext.HttpContext.Request.UserAgent,
                filterContext.HttpContext.Request.UrlReferrer == null ? string.Empty : filterContext.HttpContext.Request.UrlReferrer.ToString(),
                GetIP(filterContext)
                );
            LogManager.GetLogger(typeof(SilenceHandleErrorAttribute))
                            .Error(msg, filterContext.Exception);
            
            if (filterContext.IsChildAction)
            {
                return;
            }
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }
            
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult { 
                ViewName = "Error"
            };
            
            
        }
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        private string GetIP(ExceptionContext context)
        {
            string result = String.Empty;
            result = context.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
            {
                result = context.HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = context.HttpContext.Request.UserHostAddress;
            }
            if (string.IsNullOrEmpty(result))
            {
                return "127.0.0.1";
            }
            return result;
        }
        #endregion
    }
}