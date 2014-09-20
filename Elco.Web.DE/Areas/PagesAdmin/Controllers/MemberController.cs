using System.Web.Mvc;

using Elco.Services;
using Elco.Models;
using Elco.Mvc.Filters;
using Controleng.Common;
using System.Data;
using System;

namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class MemberController : Controller
    {
        //
        // GET: /PagesAdmin/Member/

        #region == 用户列表 ==
        public ActionResult List()
        {

            var list = MemberService.List(new SearchSetting() { 
                PageIndex = Controleng.Common.CECRequest.GetQueryInt("page",1)
            });
            ViewBag.List = list;


            return View();
        }
        #endregion

        #region == 下载 ==
        public ActionResult Download(FormCollection fc)
        {
            var monthList = MemberService.MonthList();
            ViewBag.MonthList = monthList;

            if (Request.HttpMethod == "POST")
            {
                DataTable dt = MemberService.ListByMonth(fc["month"]);
                Output(dt);
            }

            return View();
        }
        private void Output(System.Data.DataTable oTable)
        {
            Response.Clear();
            Response.ContentType = "application/ms-excel";
            Response.AppendHeader("Content-Disposition", string.Format("attachment;filename=member_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmm")));
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            foreach (System.Data.DataColumn dc in oTable.Columns)
            {
                Response.Write(dc.ColumnName.ToLower() + ",");
            }

            Response.Write("\n");


            foreach (System.Data.DataRow dr in oTable.Rows)
            {
                foreach (System.Data.DataColumn dc in oTable.Columns)
                {
                    string s = dr[dc].ToString().Replace(",", " ");
                    s = System.Text.RegularExpressions.Regex.Replace(s, @"[\f\n\r\t\v]", string.Empty);
                    Response.Write(s + ",");
                }
                Response.Write("\n");
            }
            Response.End();
        }
        #endregion

        #region == 管理员列表  ==
        /// <summary>
        /// 管理员列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminList() {

            if(Request.HttpMethod.ToUpper() == "POST"){
                string _action = CECRequest.GetFormString("_action").ToLower();
                if(_action == "search"){
                    //查询
                    string _userName = CECRequest.GetFormString("txtUserName");
                    if(!string.IsNullOrEmpty(_userName)){
                        ViewBag.SearchList = MemberService.SearchAdmin(_userName);
                    }
                }

                if(_action == "add"){
                    int _userId = CECRequest.GetFormInt("_userId", 0);
                    bool isSuccess = MemberService.AddAdmin(_userId);
                    if (isSuccess)
                    {
                        ViewBag.Status = "AddSuccess";
                    }
                    else {
                        ViewBag.Status = "AddError";
                    }
                }

                //判断是否为删除                
                if(_action == "delete"){
                    int _userId = CECRequest.GetFormInt("_userId",0);
                    MemberService.DeleteAdmin(_userId);
                    ViewBag.Status = "DeleteSuccess";
                }
            }
            var list = MemberService.AdminList();

            ViewBag.List = list;

            return View();
        }
        #endregion

        #region == 修改密码 =
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult SetAdminPwd() {
            return View();
        }
        [HttpPost]
        public ActionResult SetAdminPwd(FormCollection fc) {
            bool errors = false;
            string oldPassword = fc["txtOldPassword"];
            string newPassword = fc["txtNewPassword"];
            if(string.IsNullOrEmpty(oldPassword)){
                ModelState.AddModelError("oldPassword","请输入旧密码！");
                errors = true;
            }
            if(string.IsNullOrEmpty(newPassword)){
                ModelState.AddModelError("newPassword", "请输入新密码！");
                errors = true;
            }
            if(!errors && ModelState.IsValid){
                
                //首先验证原密码是否争取
                var userInfo = ElcoHttpContext.Current.AdminUserInfo;
                if (userInfo.Id > 0)
                {
                    if (MemberService.ValidateAdmin(userInfo.UserName, oldPassword))
                    {
                        //正确
                        MemberService.SetPassword(userInfo.Id, newPassword);
                        ViewBag.Status = "OK";
                    }
                    else
                    {
                        ModelState.AddModelError("oldPasswordError", "原密码错误，请重试！");
                    }
                }
            }
            return View();
        }
        #endregion

    }
}
