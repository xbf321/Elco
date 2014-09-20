using System.Web.Mvc;
using System.Linq;

using Elco.Services;
using Elco.Config;
using Elco.Models;
using Controleng.Common;


namespace Elco.Web.WWW.Controllers
{
    /// <summary>
    /// 招贤纳士
    /// </summary>
    public class JobController : BaseController
    {
        //
        // GET: /Job/

        public ActionResult Index()
        {
            /*
             * 设置_CommmonLayout.cshtml中的变量
             */
            var root = CategoryService.Get(GeneralConfig.JobRootId);
            var first = root;
            if(root.IsShowFirstChildNode){
                var _first = CategoryService.ListByParentId(root.Id).Where(p => (p.IsDeleted == false && p.IsEnabled == true)).FirstOrDefault();
                if (_first != null)
                {
                    first = _first;
                }
            }
            ViewBag.RootCategoryInfo = root;
            ViewBag.CurrentCategoryInfo = first;

            var list = JobService.List(new JobSearchSetting() { 
                PageIndex = CECRequest.GetQueryInt("page",1),
                IsOnlyShowPublished = true
            });

            ViewBag.List = list;

            return View();
        }
        [ValidateInput(false)]
        public ActionResult Apply(ResumeInfo model,FormCollection fc) {

            /*
             * 设置_CommmonLayout.cshtml中的变量
             */

            ViewBag.RootCategoryInfo = ViewBag.CurrentCategoryInfo = CategoryService.Get(GeneralConfig.JobRootId);

            int jobId = CECRequest.GetQueryInt("jobId",0);

            var jobInfo = JobService.Get(jobId);
            if(jobInfo.Id<=0){
                return Redirect("/job");
            }

            if(Request.HttpMethod.ToUpper() == "POST"){

                //生日
                model.Birthday = string.Format("{0}-{1}",fc["Year"],fc["Month"]);

                //籍贯
                string birthplace = string.Empty;
                string province = fc["Province"],city = fc["City"],town = fc["Town"];
                if(!string.IsNullOrEmpty(province)){
                    birthplace = province;
                    if(!string.IsNullOrEmpty(city)){
                        birthplace += "-" + city;
                        if(!string.IsNullOrEmpty(town)){
                            birthplace += "-" + town;
                        }
                    }
                }
                model.BirthPlace = birthplace;

                int id = ResumeService.Create(model).Id;
                bool status = false;
                if(id >0){
                    status = true;
                }
                //POST
                ViewBag.Status = status;    
            }

            ViewBag.JobInfo = jobInfo;
            return View(model);
        }
        

    }
}
