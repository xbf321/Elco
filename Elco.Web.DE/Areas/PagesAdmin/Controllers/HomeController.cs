using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

using Elco.Models;
using Elco.Mvc.Filters;
using Elco.Services;

namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /PagesAdmin/Home/

        [PagesAdminAuth]
        public ActionResult Index()
        {
            return View();
        }

        #region == 登录 ==
        public ActionResult Login() {

            //首先清除Cookie和前台公用一个Cookie名称
            ElcoHttpContext.Current.AdminLogout();

            return View();
        }
        [HttpPost]
        public ActionResult Login(MemberInfo model) {

            bool errors = false;
            if(string.IsNullOrEmpty(model.UserName)){
                ModelState.AddModelError("UserNameEmpty", "Please enter your user name.");
                errors = true;
            }
            if(string.IsNullOrEmpty(model.UserPassword)){
                errors = true;
                ModelState.AddModelError("UserPasswordEmpty", "Please enter a password.");
            }
            if (!errors && ModelState.IsValid)
            {
                if(MemberService.ValidateAdmin(model.UserName,model.UserPassword)){
                    var memberInfo = MemberService.Get(model.UserName);
                    ElcoHttpContext.Current.LoginEvent(ElcoHttpContext.Current.AdminCookieName,memberInfo);
                    return Redirect("/pagesadmin/");
                }
                ModelState.AddModelError("LoginError", "User name or password is incorrect, please try again.");               
                
            }
            return View();
        }
        
        #endregion

        #region == 登出 ==
        public ActionResult Logout() {
            //清除Cookie
            ElcoHttpContext.Current.AdminLogout();
            return Redirect("/pagesadmin/login");
        }
        #endregion

        #region == 上传图片 ==
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadImage()
        {
            string imageUrl = SaveImageToFile();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ViewBag.Msg = "success";
                ViewBag.ImageUrl = imageUrl;
            }
            return View();
        }
        #endregion

        #region == 异步上传图片 ==
        [HttpPost]
        public void AjaxUploadImage()
        {
            Response.Write(SaveImageToFile());
            Response.End();
        }
        #endregion

        #region == 保存图片到硬盘 ==
        private string SaveImageToFile()
        {
            string returnImage = string.Empty;
            var folder = string.Format("{0}{1}",Server.MapPath("~"),"\\Upload\\");
            HttpFileCollectionBase files = Request.Files;
            try
            {
                HttpPostedFileBase postedFile = files[0];
                //
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension);
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    returnImage = string.Format("/Upload/{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);
                }
            }
            catch
            {
                //Logger.Error(ex.ToString());
            }
            finally
            {
                files = null;
            }
            return returnImage;
        }
        #endregion

        #region == 上传附件 ==
        [HttpPost]
        public void UploadSWF()
        {
            var folder = string.Format("{0}{1}", Server.MapPath("~"), "\\Upload\\");
            HttpFileCollectionBase files = Request.Files;
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                HttpPostedFileBase postedFile = files["attachFile"];
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{2}_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension, originalFileName.Replace(originalExtension, string.Empty));
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    /*
                    //以流模式保存文件
                    // 把 Stream 转换成 byte[]
                    byte[] bytes = new byte[postedFile.InputStream.Length];
                    postedFile.InputStream.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始
                    postedFile.InputStream.Seek(0, SeekOrigin.Begin);
                    // 把 byte[] 写入文件
                    fs = new FileStream(String.Concat(imageServerFolder, newFileName), FileMode.Create);
                    bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();*/


                    string returnImage = string.Format("/Upload/{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);
                    Response.StatusCode = 200;
                    Response.Write(returnImage);
                }
                Response.Write("");

            }
            catch
            {
                //Logger.Error(ex.ToString());
                Response.StatusCode = 500;
                Response.Write("An error occured");
                Response.End();
            }
            finally
            {
                if (fs != null) { fs.Close(); fs.Dispose(); }
                if (bw != null) { bw.Close(); bw.Dispose(); }
                Response.End();
            }
        }
        #endregion
    }
}
