using System;
using System.Web;
using System.Web.Mvc;

using Elco.Models;
using Elco.Services;

namespace Elco.Web.WWW.Controllers
{
    public class PassportController : BaseController
    {
        //
        // GET: /Passport/
        #region == 注册  ==
        public ActionResult Register() {
            
            return View();
        }
        [HttpPost]
        public ActionResult Register(MemberInfo model) {

            bool errors = false;

            if(string.IsNullOrEmpty(model.UserName)){
                errors = true;
                ModelState.AddModelError("UserName","用户名不能为空");
            }
            if(string.IsNullOrEmpty(model.UserPassword)){
                errors = true;
                ModelState.AddModelError("UserPassword","密码不能为空");
            }
            if(!errors && ModelState.IsValid){
                //验证用户名是否存在
                errors = MemberService.Get(model.UserName).Id>0;
                if (!errors)
                {
                    //验证Email是否存在
                    if (MemberService.EmailExists(model.Email))
                    {
                        ModelState.AddModelError("EmailExists", "Email已经存在，请选择其他Email。");
                    }
                    else
                    {
                        MemberService.Create(model);
                        //显示注册成功页面
                        return View("RegisterSuccess", model);
                    }
                }
                else {
                    ViewBag.Status = "UserNameExists";
                }
            }

            return View(model);
        }
        #endregion

        #region == 登陆 ==
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(MemberInfo model,FormCollection fc){
            bool errors = false;
            
            if (string.IsNullOrEmpty(model.UserName))
            {
                errors = true;
                ModelState.AddModelError("UserName", "用户名不能为空");
            }
            if (string.IsNullOrEmpty(model.UserPassword))
            {
                errors = true;
                ModelState.AddModelError("UserPassword", "密码不能为空");
            }
            if(!errors && ModelState.IsValid){
                var userInfo = MemberService.Get(model.UserName);
                if (userInfo.UserPassword == model.UserPassword)
                {

                    ElcoHttpContext.Current.LoginEvent(ElcoHttpContext.Current.CookieName, userInfo);

                    return Redirect("/download");
                }
                else {
                    ViewBag.Status = "NotLogined";
                }
            }            
            return View();
        }
        /// <summary>
        /// mini登录框
        /// </summary>
        /// <returns></returns>
        public ActionResult MiniLogin() {
            return View();
        }
        [HttpPost]
        public ActionResult MiniLogin(MemberInfo model,FormCollection fc) {
            bool errors = false;

            if (string.IsNullOrEmpty(model.UserName))
            {
                errors = true;
                ModelState.AddModelError("UserName", "用户名不能为空");
            }
            if (string.IsNullOrEmpty(model.UserPassword))
            {
                errors = true;
                ModelState.AddModelError("UserPassword", "密码不能为空");
            }
            if (!errors && ModelState.IsValid)
            {
                var userInfo = MemberService.Get(model.UserName);
                if (userInfo.UserPassword == model.UserPassword)
                {
                    ElcoHttpContext.Current.LoginEvent(ElcoHttpContext.Current.CookieName,userInfo);

                    ViewBag.Status = "OK";
                }
                else
                {
                    ViewBag.Status = "NotLogined";
                }
            }
            return View();
        }
        #endregion

        #region == 登出 ==
        public ActionResult Logout() {
            ElcoHttpContext.Current.Logout();

            string returnUrl = "/";
            if (Request.UrlReferrer != null)
            {
                returnUrl = Request.UrlReferrer.AbsolutePath;
            }
            
            return Redirect(returnUrl);
        }
        #endregion

    }
}
