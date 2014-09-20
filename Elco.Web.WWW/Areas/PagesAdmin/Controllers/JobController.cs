using System.Web.Mvc;

using Elco.Models;
using Controleng.Common;
using Elco.Services;
using Elco.Mvc.Filters;

namespace Elco.Web.WWW.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class JobController : Controller
    {
        //
        // GET: /PagesAdmin/Job/

        #region == 添加 或 编辑 ==
        public ActionResult Create()
        {
            int id = CECRequest.GetQueryInt("id",0);
            var model = JobService.Get(id);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(JobInfo model) {

            bool errors = false;
            bool isAdd = model.Id == 0;

            if(string.IsNullOrEmpty(model.Title)){
                errors = true;
                ModelState.AddModelError("Title","职位名称不能为空");
            }
            if(!errors && ModelState.IsValid){
              
                int id = JobService.Create(model).Id;
                ViewBag.Status = false;
                if(id>0){
                    ViewBag.Status = true;
                }
            }
            return View(model);
        }
        #endregion

        #region == 列表 ==
        public ActionResult List() {

            if(Request.HttpMethod.ToUpper() == "POST"){
                if(CECRequest.GetFormString("action").ToLower() == "del"){
                    //删除
                    int id = CECRequest.GetFormInt("id",0);
                    JobService.Delete(id);
                }
                if (CECRequest.GetFormString("action").ToLower() == "pub") { 
                    //发布
                    int id = CECRequest.GetFormInt("id",0);
                    JobService.SetPublish(id,true);
                }
            }

            var list = JobService.List(new JobSearchSetting()
            { 
                PageSize = 15,
                PageIndex = CECRequest.GetQueryInt("page",1),
                IsOnlyShowPublished = false //false 全部显示
            });
            ViewBag.List = list;
            return View();
        }
        #endregion

        public ActionResult ResumeList() {

            if(Request.HttpMethod.ToUpper() == "POST"){
                if(CECRequest.GetFormString("_action").ToLower() == "del"){
                    int id = CECRequest.GetFormInt("_id",0);
                    ResumeService.Delete(id);
                }
            }

            var list = ResumeService.List(new SearchSetting() { 
                PageIndex = CECRequest.GetQueryInt("page",1)
            });

            ViewBag.List = list;

            return View();
        }

    }
}
