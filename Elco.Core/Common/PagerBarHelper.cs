using System.Text;
using System.Reflection;
using System.Web;

namespace Elco.Common
{
    public static class PagerBarHelper
    {
        public static string Render(int pageIndex, int displaySize, int totalNumber) {
            return Render(pageIndex,displaySize,totalNumber,null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="displaySize"></param>
        /// <param name="totalNumber"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="lang">0:中文，1:英文</param>
        /// <returns></returns>
        public static string Render(int pageIndex,int displaySize,int totalNumber,object htmlAttributes,int lang = 0){
            if (pageIndex <= 0) { pageIndex = 1; }
            int pages = totalNumber % displaySize == 0 ? totalNumber / displaySize : totalNumber /displaySize +1;

            if (pageIndex >= pages) { pageIndex = pages; }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div");
            if(htmlAttributes != null){
                foreach(PropertyInfo pi in htmlAttributes.GetType().GetProperties()){
                    sb.AppendFormat(" {0}=\"{1}\"",pi.Name,pi.GetValue(htmlAttributes,null));
                }
            }
            sb.Append(">");
            if(lang == 0){
                sb.AppendFormat("共<em>{0}</em>条记录,&nbsp;",totalNumber);
            }else{
                sb.AppendFormat("Total:{0},&nbsp;",totalNumber);
            }
            sb.AppendFormat("<em>{0}</em>&nbsp;/&nbsp;{1}&nbsp;",pageIndex, pages);
            if(pageIndex != 1){
                if (totalNumber > 0)
                {
                    //显示第一页
                    sb.AppendFormat("<span>{0}</span>", BuildUrl(1, lang == 0 ? "第一页" : "First"));
                }
            }
            if (pageIndex - 1 > 0)
            {
                sb.AppendFormat("<span>{0}</span>", BuildUrl(pageIndex - 1, lang == 0? "上一页":"Pre"));
            }
            else {
                //sb.Append("<span>上一页</span>");
            }
            if(pageIndex +1 <=pages){
                sb.AppendFormat("<span>{0}</span>", BuildUrl(pageIndex + 1, lang == 0 ? "下一页" : "Next"));
            }
            else
            {
                //sb.Append("<span>下一页</span>");
            }
            
            if(pageIndex != pages){
                //显示最后一页
                sb.AppendFormat("<span>{0}</span>",BuildUrl(pages, lang == 0 ? "最后一页" : "Last"));
            }
            sb.Append("</div>");
            return sb.ToString();
        }
        private static string BuildUrl(int pageIndex,string text) {
            string localPath = HttpContext.Current.Request.Url.LocalPath;
            StringBuilder sbLocalPath = new StringBuilder(localPath);
            sbLocalPath.Append("?");
            var querys = HttpContext.Current.Request.QueryString;
            if (querys != null)
            {
                foreach (string key in querys.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && key.Equals("page")) { continue; }
                    string value = HttpUtility.UrlEncode(querys[key]);
                    sbLocalPath.AppendFormat("{0}{1}&", string.IsNullOrEmpty(key) ? string.Empty : string.Concat(key, "="), value);
                }
            }
            sbLocalPath.AppendFormat("page={0}", pageIndex);
            return string.Format("<a href=\"{0}\" target=\"_self\">{1}</a>",sbLocalPath.ToString(),text);
        }
    }
}
