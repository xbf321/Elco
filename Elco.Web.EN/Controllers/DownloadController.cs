using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elco.Services;
using Elco.Config;
using Elco.Models;
using Controleng.Common;
using System.Text;

namespace Elco.Web.En.Controllers
{
    /// <summary>
    /// 下载中心
    /// </summary>
    public class DownloadController : BaseController
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {

            int catId = CECRequest.GetQueryInt("catId",0);

            if (catId > 0)
            {
                ViewBag.CurrentCategoryInfo = CategoryService.Get(catId);
            }

            var list = AttachmentService.List(new AttachmentSearchSetting()
            { 
                PageIndex = CECRequest.GetQueryInt("page",1),
                CategoryId = catId
            });

            ViewBag.List = list;

            return View();
        }

        #region == 追踪 ==
        /// <summary>
        /// 追踪
        /// </summary>
        /// <returns></returns>
        public ActionResult Track() {
            if (!ElcoHttpContext.Current.IsLogin) {

                return Content("<script>alert('Please login!');window.location.href = '/passport/login';</script>");
            }
            var userInfo = ElcoHttpContext.Current.LoginUserInfo;
            
            var guid = CECRequest.GetQueryString("guid");
            var info = AttachmentService.GetByGUID(guid);
            if(info.Id>0){

                //更新Attachment下载次数
                AttachmentService.UpdateDownloadCount(info.Id);

                //记录AttachmentDownloadLog信息
                AttachmentService.InsertLog(new AttachmentDownloadLogInfo() { 
                    AttachId = info.Id,
                    UserId = userInfo.Id,
                    UserName = userInfo.UserName
                });

                //调转到下载地址
                return Redirect(info.DownloadUrl.TrimEnd());
            }
            return Content(string.Empty);
        }
        #endregion
    }
}
