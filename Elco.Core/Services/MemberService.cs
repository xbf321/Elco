
using Elco.Models;
using Elco.Data;
using System.Collections.Generic;
using System;
using System.Data;

namespace Elco.Services
{
    public static class MemberService
    {
        /// <summary>
        /// 插入或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static MemberInfo Create(MemberInfo model) {
            if (model.Id == 0)
            {
                int id = MemberManage.Insert(model);
                model.Id = id;
            }
            else {
                MemberManage.Update(model);
            }
                return model;
        }/// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<MemberInfo> List(SearchSetting setting){
            return MemberManage.List(setting);
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MemberInfo Get(int id) {
            return MemberManage.Get(id);
        }
        public static MemberInfo Get(string userName) {
            return MemberManage.Get(userName);
        }
        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static bool Validate(string userName,string userPassword) {
            return MemberManage.Validate(userName,userPassword);
        }
        /// <summary>
        /// Email地址是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool EmailExists(string email) {
            return MemberManage.EmailExists(email);
        }

        public static bool SetPassword(int userId,string newPassword) {
            return MemberManage.UpdatePassword(userId,newPassword);
        }
        public static List<Tuple<string, int>> MonthList() {
            return MemberManage.MonthList();
        }
        /// <summary>
        /// 获得每月的用户数据
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable ListByMonth(string month) {
            return MemberManage.ListByMonth(month);
        }

        #region == 后台用户 ==
        /// <summary>
        /// 查询管理员
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SearchAdmin(string userName) {
            return MemberManage.SearchAdmin(userName);
        }
         /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool AddAdmin(int userId) {
            return MemberManage.AddAdmin(userId);
        }
        /// <summary>
        /// 验证是否为后台管理员
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static bool ValidateAdmin(string userName, string userPassword) {
            return MemberManage.ValidateAdmin(userName,userPassword);
        }
        /// <summary>
        /// 后台管理员列表
        /// </summary>
        /// <returns></returns>
        public static List<MemberInfo> AdminList() {
            return MemberManage.AdminList();
        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="userId"></param>
        public static void DeleteAdmin(int userId) {
            MemberManage.DeleteAdmin(userId);
        }
        #endregion
    }
}
