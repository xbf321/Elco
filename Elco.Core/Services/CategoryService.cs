using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class CategoryService
    {
        #region == Edit Or Add ==
        /// <summary>
        /// 添加或更新分类信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns>主键ID</returns>
        public static int Create(CategoryInfo model)
        {
            if (model.Id == 0)
            {
                //Insert
                int i = CategoryManage.Insert(model);
                model.Id = i;
            }
            else
            {
                //Update
                CategoryManage.Update(model);
            }
            return model.Id;
        }
        #endregion

        #region == Delete ==
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void Delete(int id) {
            CategoryManage.Delete(id);
        }
        #endregion

        #region == Restore ==
        /// <summary>
        /// 恢复分类
        /// </summary>
        /// <param name="id"></param>
        public static void Restore(int id) {
            CategoryManage.Restore(id);
        }
        #endregion

        #region == Enable OR Disable ==
        public static void Enable(int id) {
            CategoryManage.Enable(id);
        }
        #endregion

        #region == 获得此ID的类别详细信息==
        /// <summary>
        /// 获得此ID的类别详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CategoryInfo Get(int id)
        {
            var item =  CategoryManage.Get(id);
            LoadExtionsion(item);
            return item;
        }
        #endregion

        #region == 分类名是否存在 ==
        /// <summary>
        ///分类名是否存在，在同一语言下
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static bool ExistsName(int id, string name, int parentId)
        {
            return CategoryManage.ExistsName(id, name, parentId);
        }
        #endregion

        #region == 分类别名是否存在 ==
        /// <summary>
       /// 是否存在别名，别名不允许重复
       /// </summary>
       /// <param name="appId"></param>
       /// <param name="cid"></param>
       /// <param name="englishName"></param>
       /// <returns></returns>
        public static bool ExistsAlias(int cid,string englishName) {
            if (string.IsNullOrEmpty(englishName)) { return false; }
            return CategoryManage.ExistsAlias(cid,englishName);
        }
        #endregion

        #region == 获得全部分类,包含删除的和未启用的 ==
        /// <summary>
        /// 获得全部分类,包含删除的和未启用的
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListAll()
        {
            var list = CategoryManage.ListAll();
            foreach (var item in list)
            {
                LoadExtionsion(item);
            }
            return list;
        }
        #endregion

        #region == 根据父ID获取此ID下一级栏目，只显示一级子分类，包含删除的和未启用的 ==
        /// <summary>
        /// 根据父ID获取此ID下一级栏目，只显示一级子分类，包含删除的和未启用的
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListByParentId(int parentId,bool showEnabled = false,bool showDeleted = true,WebLanguage language = WebLanguage.zh_cn) {
            var _list = CategoryManage.ListByParentId(parentId).AsEnumerable();
            if(showEnabled){
                _list = _list.Where(p => p.IsEnabled == true);
            }
            
            if(!showDeleted){
                _list = _list.Where(p=>p.IsDeleted == false);
            }

            foreach (var item in _list)
            {
                LoadExtionsion(item);
            }
            return _list.ToList();
        }
        #endregion

        #region == 生成类别URL ==
        /// <summary>
        /// 生成类别URL
        /// </summary>
        /// <param name="catInfo"></param>
        private static void LoadExtionsion(CategoryInfo catInfo) {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(catInfo.LinkUrl))
            {
                sb.AppendFormat("/channel/{0}.html",
                         string.IsNullOrEmpty(catInfo.Alias) ? catInfo.Id.ToString() : catInfo.Alias);

            }
            else
            {
                //直接连接
                sb.AppendFormat("{0}", catInfo.LinkUrl);
            }
            catInfo.Url = sb.ToString();

        }
        #endregion

        #region == 根据此ID获得所有祖先节点，正序排列，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有祖先，正序排列，包括此节点，包含未启用的
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang">首先根据语言缩小查找范围</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListUpById(int id) {
            IList<CategoryInfo> upList = new List<CategoryInfo>();
            //向上遍历则包含删除的和未启用的
            var listAll = ListAll();
            BuildListUpByParentId(listAll,id,ref upList);
            upList = upList.Reverse().ToList();
            foreach(var item in upList){
                LoadExtionsion(item);
            }
            return upList;
        }
        private static void BuildListUpByParentId(IEnumerable<CategoryInfo> list, int parentId, ref IList<CategoryInfo> upList)
        {
            var item = list.Where(p => p.Id == parentId).FirstOrDefault();
            if (item != null && item.Id > 0)
            {
                upList.Add(item);
                if (item.ParentId > 0)
                {
                    BuildListUpByParentId(list, item.ParentId, ref upList);
                }
            }
        }
        #endregion

        #region == 根据此ID获得所有孩子节点，正序排列，包括此节点【递归实现】 ==
        /// <summary>
        /// 根据此ID获得所有孩子节点，包括此节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang">首先根据语言缩小查找范围</param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListDownById(int id) {
            IList<CategoryInfo> downList = new List<CategoryInfo>();

            //向下遍历则去掉删除的和未启用的
            var listAll = ListAll().Where(p=>(!p.IsDeleted && p.IsEnabled));

            //添加此节点本身
            downList.Add(listAll.Where( p => p.Id == id).FirstOrDefault());

            BuildListDownByParentId(listAll, id, ref downList);
            foreach(var item in downList){
                LoadExtionsion(item);
            }
            return downList;
        }
        private static void BuildListDownByParentId(IEnumerable<CategoryInfo> list, int parentId, ref IList<CategoryInfo> downList)
        {
            var itemList = list.Where(p=>p.ParentId == parentId);
            if (itemList != null && itemList.Count() > 0)
            {
                foreach (var item in itemList) {
                    downList.Add(item);
                    BuildListDownByParentId(list, item.Id, ref downList);
                }
                
            }
        }
        #endregion        
        
        #region == 生成带有层级的树列表 ==
        /// <summary>
        /// 生成带有层级的树列表
        /// </summary>
        /// <param name="newList">新列表</param>
        /// <param name="oldList">原始数据</param>
        /// <param name="parentId">指定父类别</param>
        public static void BuildListForTree(List<CategoryInfo> newList, List<CategoryInfo> oldList, int parentId)
        {
            var plist = oldList.FindAll(nc => nc.ParentId == parentId);
            if (plist.Count == 0) { return; }
            foreach (var item in plist)
            {
                if (item.ParentId == 0)
                {
                    newList.Add(item);
                }
                int index = newList.FindIndex(delegate(CategoryInfo m) { return m.Id == item.ParentId; });
                if (index > -1)
                {
                    #region 判断level

                    int level = 0;
                    CategoryInfo ncTmp = newList.Find(x => x.Id == item.ParentId);
                    while (ncTmp != null)
                    {
                        ncTmp = newList.Find(x => x.Id == ncTmp.ParentId);
                        level++;
                    }
                    #endregion

                    #region 插入到父级索引后

                    index += 1;
                    //如果紧跟父级的项是属于该父级的子级或者子级的子级……(递归下去)
                    while (newList.Count > index && CompareParentID(newList, newList[index], item.ParentId))
                    {
                        //则插入到该子级索引后
                        index += 1;
                    }
                    item.Name = BuildLevelString(level) + item.Name;
                    newList.Insert(index, item);
                    #endregion
                }
                BuildListForTree(newList, oldList, item.Id);
            }
        }
        /// <summary>
        /// 判断某子项的所有父项中是否存在指定父ID
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="child">子项</param>
        /// <param name="compareParentId">父ID</param>
        /// <returns></returns>
        private static bool CompareParentID(List<CategoryInfo> list, CategoryInfo child, int compareParentId)
        {
            if (child.ParentId == compareParentId) return true;
            var category = list.Find(c => c.Id == child.ParentId);
            while (category != null)
            {
                if (category.ParentId == compareParentId) return true;
                var nextParentId = category.ParentId;
                category = list.Find(c => c.Id == nextParentId);
            }
            return false;
        }
        private static string BuildLevelString(int level)
        {
            StringBuilder sb = new StringBuilder();
       
                for (int i = 0; i < level; i++)
                {
                    sb.Append("─");
                }

            return sb.ToString();
        }        
        #endregion

        #region == 根据Id获得对应下所有分类（SQL实现） ==
        /// <summary>
        /// 获得对应ID下所有子分类，包含自身
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IList<CategoryInfo> ListAllSubCatById(int id) {
            var list = CategoryManage.ListAllSubCatById(id);
            foreach(var item in list){
                LoadExtionsion(item);
            }
            return list;
        }
        public static IList<CategoryInfo> ListAllSubCatById(int[] ids) {
            var list = CategoryManage.ListAllSubCatById(ids);
            foreach (var item in list)
            {
                LoadExtionsion(item);
            }
            return list;
        }
        #endregion

    }

}
