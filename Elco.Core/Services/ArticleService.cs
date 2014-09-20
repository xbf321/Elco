using System;
using System.Collections.Generic;
using System.Linq;


using Elco.Models;
using Elco.Data;
using System.Text.RegularExpressions;

namespace Elco.Services
{
    public static class ArticleService
    {
        #region == Edit OR Add ==
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Create(ArticleInfo model) {

            //处理一下关键词
            if(!string.IsNullOrEmpty(model.Keywords)){
                model.Keywords = Regex.Replace(model.Keywords, @"(\s+|，)", ",");
            }

            if (model.Id > 0)
            {
                //Update
                ArticleManage.Update(model);
                
            }
            else {
                int i = ArticleManage.Insert(model);
                model.Id = i;
            }
            return model.Id;
        }
        #endregion

        #region == List With Pager ==
        /// <summary>
        /// 查询，带分页
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ArticleInfo> List(ArticleSearchSetting setting) {
            var list = ArticleManage.List(setting);
            foreach(var item in list){
                LoadExtensionInfo(item);
            }
            return list;
        }
        #endregion

        #region == 根据文章ID,获得文章详细信息 ==
        /// <summary>
        /// 根据文章ID,获得文章详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ArticleInfo Get(int id)
        {
            var model = ArticleManage.Get(id);
            return model;
        }
        #endregion

        #region == 添加扩展信息 ==
        /// <summary>
        /// 添加扩展信息，主要生成文章URL
        /// </summary>
        /// <param name="model"></param>
        /// <param name="language"></param>
        private static void LoadExtensionInfo(ArticleInfo model) {

            if (!string.IsNullOrEmpty(model.LinkUrl))
            {
                model.Url = model.LinkUrl;
            }
            else
            {
                model.Url = string.Format("/n/{0}.html",model.FullTimespan);
            }
        }
        #endregion

        #region == Update ViewCount ==
        /// <summary>
        /// 更新浏览数
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateViewCount(int id) {
            ArticleManage.UpdateViewCount(id);
        }
        #endregion

        #region == 根据文章URl，获得文章详细信息 ==
        /// <summary>
        /// 根据GUID获得文章详细信息
        /// </summary>
        /// <param name="guid">GUID</param>
        /// <param name="language">语言，主要生成URL用</param>
        /// <returns></returns>
        public static ArticleInfo GetByFullTimespan(string guid) { 
            var model = ArticleManage.GetByFullTimespan(guid);
            LoadExtensionInfo(model);
            return model;
        }


        #endregion

        #region == 查询,暂时没用 ==
        /// <summary>
        /// 查询页面所调用方法,获取所有的数据
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //public static List<ArticleInfo> Seek(int siteId, string key)
        //{
        //    var list = ArticleManage.Seek(key);
        //    foreach(var item in list){
        //        LoadExtensionInfo(item);
        //    }
        //    return list;
        //}
        #endregion

        #region == 获得某一站点所有文章的发布时间，主要为后台查询文章用,暂时没用 ==
        /// <summary>
        /// 获得某一站点所有文章的发布时间，主要为后台查询文章用
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //public static List<Tuple<string, string>> GetAllPublishDateBySiteId(int siteId) {
        //    //return ArticleManage.GetAllPublishDateBySiteId(siteId);
        //    return null;
        //}
        #endregion

        #region == 设置文章发布状态 ==
        /// <summary>
        /// 设置文章发布状态
        /// </summary>
        /// <param name="status"></param>
        public static bool SetPublishStatus(int id, bool status) {
            return ArticleManage.SetPublishStatus(id,status);
        }
        #endregion

        #region == 获得文章列表(无分页) ==
        /// <summary>
        /// 获得文章列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount">默认10条</param>
        /// <returns></returns>
        public static List<ArticleInfo> ListWithoutPage(int categoryId, int topCount, bool isTopOneImg = false){
            var list = ArticleManage.ListWithoutPage(categoryId,topCount,isTopOneImg);
            foreach (var item in list)
            {
                LoadExtensionInfo(item);
            }
            return list;
        }
        #endregion
    }
}
