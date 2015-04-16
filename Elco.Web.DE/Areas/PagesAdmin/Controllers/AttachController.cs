using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;

using Elco.Models;
using Controleng.Common;
using Elco.Services;
using Elco.Mvc.Filters;
using Elco.Config;
using System.Text;


namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class AttachController : Controller
    {
        //
        // GET: /PagesAdmin/Attach/

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {

            if (Request.HttpMethod.ToUpper() == "POST")
            {
                if (CECRequest.GetFormString("action").ToLower() == "del")
                {
                    //删除
                    int id = CECRequest.GetFormInt("id", 0);
                    AttachmentService.Delete(id);
                }
            }

            int catId = CECRequest.GetQueryInt("catId",0);
            if(catId == 0){
                catId = Elco.Config.GeneralConfig.DownloadRootId_DE;
            }

            var list = AttachmentService.List(new AttachmentSearchSetting()
            { 
                PageIndex = CECRequest.GetQueryInt("page",1),
                PageSize = 15,
                CategoryId = catId
            });
            ViewBag.List = list;

            

            return View();
        }

        #region == 添加 或 编辑 ==
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Create() {
            int id = CECRequest.GetQueryInt("id",0);
            var model = AttachmentService.Get(id);

            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AttachmentInfo model,FormCollection fc) {
            bool errors = false;
            if(string.IsNullOrEmpty(model.Title)){
                errors = true;
                ModelState.AddModelError("Title", "Please enter the name of the attachment");
            }
            if(!errors && ModelState.IsValid){

                //分类
                model.CategoryId = Utils.StrToInt(fc["ddlCats"], 0);
                if (model.CategoryId == 0)
                {
                    ModelState.AddModelError("CategoryError", "Please select the category");
                }
                else
                {

                    int id = AttachmentService.Create(model).Id;
                    ViewBag.Status = false;
                    if (id > 0)
                    {
                        ViewBag.Status = true;
                    }
                }
            }

            return View(model);
        }
        #endregion

        #region == 下载日志列表 ==
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <returns></returns>
        public ActionResult LogList() {
            var list = AttachmentService.ListLog(CECRequest.GetQueryInt("attachId",0), new SearchSetting() { 
                PageIndex = CECRequest.GetQueryInt("page",1),
                PageSize = 15
            });
            ViewBag.List = list;
            return View();
        }
        #endregion

        #region == 输出分类下拉列表 ChildActionOnly ==
        [ChildActionOnly]
        public ActionResult RenderCategoryDropdown(string name,object selectedValue) {
            //输出下载所属分类
            int rootId = GeneralConfig.DownloadRootId_DE;
            var oldCatList = CategoryService.ListAllSubCatById(rootId).Where(p => p.IsEnabled == true && p.IsDeleted == false);
            var newCatList = new List<CategoryInfo>();

            Action<IEnumerable<CategoryInfo>, List<CategoryInfo>, int, int> fb = null;
            fb = (oldList, newList, parentId, depath) =>
            {
                var tempList = oldList.Where(p => p.ParentId == parentId);
                foreach (var item in tempList)
                {
                    item.Name = string.Format("{0}├{1}", new string('│', depath), item.Name);
                    newList.Add(item);
                    var ishasChild = oldList.Where(p => p.ParentId == item.Id).Count() > 0;
                    if (ishasChild)
                    {
                        fb(oldList, newList, item.Id, (depath + 1));
                    }
                }
            };
            fb(oldCatList, newCatList, rootId, 0);

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<select id=\"{0}\" name=\"{0}\">",name);
            sbHtml.Append("<option value=\"0\">==please select==</option>");
            foreach(var item in newCatList){
                sbHtml.AppendFormat("<option value=\"{1}\" {2}>{0}</option>", item.Name, item.Id, selectedValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty);
            }
            sbHtml.Append("</select>");
            return Content(sbHtml.ToString());
        }
        #endregion
    }
}
