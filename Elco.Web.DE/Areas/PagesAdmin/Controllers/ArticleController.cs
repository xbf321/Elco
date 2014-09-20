using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;


using Elco.Services;
using Elco.Models;
using Controleng.Common;
using Elco.Mvc.Filters;
using Elco.Config;
using System;
using System.Text;
using Elco.Common;
using System.Data;

namespace Elco.Web.En.Areas.PagesAdmin.Controllers
{
    [PagesAdminAuth]
    public class ArticleController : Controller
    {
        #region == Create OR Edit ==
        public ActionResult Create()
        {
            int id = Controleng.Common.CECRequest.GetQueryInt("id", 0);
            ArticleInfo model = ArticleService.Get(id);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ArticleInfo articleInfo)
        {
            bool errors = false;
            bool isAdd = articleInfo.Id == 0 ? true : false;
            if (articleInfo.CategoryId == 0)
            {
                errors = true;
                ModelState.AddModelError("CAT", "请选择分类");
            }
            if (string.IsNullOrEmpty(articleInfo.Title))
            {
                errors = true;
                ModelState.AddModelError("TITLT", "请输入文章标题");
            }
            if (!errors && ModelState.IsValid)
            {
                ArticleService.Create(articleInfo);
                if (isAdd)
                {
                    ViewBag.Msg = "添加成功！是否继续？【<a href=\"create\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
                else
                {
                    ViewBag.Msg = "修改成功！是否继续？【<a href=\"create\">是</a>】&nbsp;&nbsp;【<a href=\"list\">否</a>】";
                }
            }

            return View(articleInfo);
        }
        #endregion

        #region == List ==
        public ActionResult List()
        {
            var articleList = ArticleService.List(new ArticleSearchSetting()
            {
                CategoryId = CECRequest.GetQueryInt("cid", 0),
                PageIndex = CECRequest.GetQueryInt("page", 1),
                Title = CECRequest.GetQueryString("title"),
                PublishDate = CECRequest.GetQueryString("m"),
                PageSize = 15,
                ShowDeleted = false
            });
            ViewBag.ArticleList = articleList;
            return View();
        }
        #endregion

        #region == 设置文章发布 ==
        [HttpPost]
        public ActionResult SetPublishStatus()
        {
            int id = CECRequest.GetFormInt("id", 0);
            bool status = ArticleService.SetPublishStatus(id, true); //设置为发布
            return Json(new { Status = status });
        }
        #endregion

        #region == 输出分类下拉列表 ChildActionOnly ==
        [ChildActionOnly]
        public ActionResult RenderCategoryDropdown(string name, object selectedValue)
        {
            //输出下载所属分类
            //输出下拉列表(排除掉产品分类和下载中心)
            var all = Elco.Services.CategoryService.ListAll().Where(p => p.IsDeleted == false);
            var nonUseCatList = CategoryService.ListAllSubCatById(new int[] { GeneralConfig.ProductRootId_EN, GeneralConfig.DownloadRootId_EN });

            var exceptCatList = all.Except(nonUseCatList);



            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat("<select id=\"{0}\" name=\"{0}\">", name);
            sbHtml.Append("<option value=\"0\">==请选择==</option>");

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
            fb(exceptCatList, newCatList, 0, 0);


            foreach (var item in newCatList)
            {
                if (item.ParentId == 0)
                {
                    sbHtml.AppendFormat("<optgroup label=\"{0}\">{0}</optgroup>", item.Name);
                }
                else
                {
                    sbHtml.AppendFormat("<option value=\"{1}\" {2}>{0}</option>", item.Name, item.Id, selectedValue.Equals(item.Id) ? "selected=\"selected\"" : string.Empty);
                }
            }


            sbHtml.Append("</select>");
            return Content(sbHtml.ToString());


        }
        #endregion
    }
}
