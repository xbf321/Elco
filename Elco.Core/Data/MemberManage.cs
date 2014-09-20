using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


using Elco.Models;
using Goodspeed.Library.Data;
using Elco.Common;

namespace Elco.Data
{
    public static class MemberManage
    {
        #region == Members ==
        /// <summary>
        /// 插入用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(MemberInfo model) {
            string strSQL = "INSERT INTO Members(UserName,UserPassword,CreateDateTime,RealName,Position,Company,Email,Phone) VALUES(@UserName,@Userpassword,GETDATE(),@RealName,@Position,@Company,@Email,@Phone);SELECT @@IDENTITY;";
            SqlParameter[] param = { 
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("UserPassword",SqlDbType.NVarChar),
                                    new SqlParameter("RealName",SqlDbType.NVarChar),
                                    new SqlParameter("Position",SqlDbType.NVarChar),
                                    new SqlParameter("Company",SqlDbType.NVarChar),
                                    new SqlParameter("Email",SqlDbType.NVarChar),
                                    new SqlParameter("Phone",SqlDbType.NVarChar),
                                   };
            param[0].Value = model.UserName ?? string.Empty;
            param[1].Value = model.UserPassword ?? string.Empty;
            param[2].Value = model.RealName ?? string.Empty;
            param[3].Value = model.Position ?? string.Empty;
            param[4].Value = model.Company ?? string.Empty;
            param[5].Value = model.Email ?? string.Empty;
            param[6].Value = model.Phone ?? string.Empty;
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 更新用户，暂时没有功能
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(MemberInfo model) {
            //string strSQL = "";
            //return SQLPlus
            return 0;
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<MemberInfo> List(SearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Members";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " CreateDateTime DESC";
            fp.WithOptions = " WITH(NOLOCK)";

            IList<MemberInfo> list = new List<MemberInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(GetByRow(dr));
                }
            }

            int count = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL())); ;
            return new PageOfList<MemberInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        /// <summary>
        /// 验证用户（用户名和密码）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static bool Validate(string userName,string userPassword) {
            string strSQL = "SELECT COUNT(*) FROM Members WITH(NOLOCK) WHERE UserName = @UserName AND UserPassword = @UserPassword";
            SqlParameter[] param = { 
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("UserPassword",SqlDbType.NVarChar),
                                   };
            param[0].Value = userName;
            param[1].Value = userPassword;
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param)) > 0;
        }
        /// <summary>
        /// 根据UserId获得用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MemberInfo Get(int id) {
            string strSQL = "SELECT * FROM Members WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter param = new SqlParameter("Id",id);
            return GetByRow(SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 根据UserName获得用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static MemberInfo Get(string userName) {
            string strSQL = "SELECT * FROM Members WITH(NOLOCK) WHERE UserName = @UserName";
            SqlParameter param = new SqlParameter("UserName", userName);
            return GetByRow(SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, param));
        }
        /// <summary>
        /// Email地址是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool EmailExists(string email) {
            string strSQL = "SELECT COUNT(*) FROM Members WHERE Email = @Email";
            SqlParameter param = new SqlParameter("Email", email);
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param))>0;
        }
        /// <summary>
        /// 填充用户详细信息
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static MemberInfo GetByRow(DataRow dr) {
            if (dr == null) { return new MemberInfo(); }
            return new MemberInfo() { 
                Id = dr.Field<int>("Id"),
                UserName = dr.Field<string>("UserName"),
                UserPassword = dr.Field<string>("UserPassword"),
                CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                RealName = dr.Field<string>("RealName"),
                Company = dr.Field<string>("Company"),
                Email = dr.Field<string>("Email"),
                Position = dr.Field<string>("Position"),
                Phone = dr.Field<string>("Phone")
            };
        }
        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool UpdatePassword(int userId,string newPassword) {
            string strSQL = "UPDATE Members SET UserPassword = @UserPassword WHERE Id = @UserId";
            SqlParameter[] param = { 
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("UserPassword",SqlDbType.NVarChar),
                                   };
            param[0].Value = userId;
            param[1].Value = newPassword;
            return SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,param) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, int>> MonthList() {
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            string strSQL = "SELECT COUNT(*) AS c,CONVERT(VARCHAR(7),CreateDateTime,120) AS m FROM dbo.Members WITH(NOLOCK) GROUP BY CONVERT(VARCHAR(7),CreateDateTime,120) ORDER BY m DESC";
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, strSQL);
            if(dt != null && dt.Rows.Count >0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(Tuple.Create(dr.Field<string>("m"),dr.Field<int>("c")));
                }
            }
            return list;
        }
        /// <summary>
        /// 获得每月的用户数据
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable ListByMonth(string month) {
            string startMonth = string.Format("{0}-01",month);
            string strSQL = string.Format("SELECT * FROM dbo.Members WITH(NOLOCK) WHERE CreateDateTime BETWEEN '{0}' AND DATEADD(MONTH,1,'{0}')",startMonth);
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, strSQL);
            return dt;
        }
        #endregion

        #region == 后台管理员操作 ==
        /// <summary>
        /// 查询管理员
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SearchAdmin(string userName) { 
            var list = new Dictionary<int,string>();
            string strSQL = "SELECT Id,UserName FROM Members WITH(NOLOCK) WHERE UserName LIKE '%'+@UserName+'%'";
            SqlParameter param = new SqlParameter("UserName",userName);
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,param);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    int userId = dr.Field<int>("Id");
                    if(!list.ContainsKey(userId)){
                        list.Add(userId,dr.Field<string>("UserName"));
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool AddAdmin(int userId) {
            string strSQL = @"DECLARE @C AS INT;
                                SELECT @C = COUNT(*) FROM PagesAdmin WITH(NOLOCK) WHERE UserId = @UserId
                                IF(@C = 0)
	                                BEGIN
		                                INSERT INTO PagesAdmin(UserId,UserName,CreateDateTime)
		                                SELECT Id,UserName,GETDATE() FROM Members WITH(NOLOCK) WHERE Id = @UserId
	                                END
                                SELECT @C";
            SqlParameter param = new SqlParameter("UserId",userId);
            int flag = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
            return flag == 0;
        }
        /// <summary>
        /// 验证是否为后台管理员
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        public static bool ValidateAdmin(string userName,string userPassword) {
            string strSQL = "SELECT COUNT(*) FROM PagesAdmin WITH(NOLOCK) INNER JOIN Members WITH(NOLOCK) ON PagesAdmin.UserId = Members.Id WHERE Members.UserName = @UserName AND Members.UserPassword = @UserPassword";
            SqlParameter[] param = { 
                                    new SqlParameter("UserName",SqlDbType.NVarChar),
                                    new SqlParameter("UserPassword",SqlDbType.NVarChar),
                                   };
            param[0].Value = userName;
            param[1].Value = userPassword;
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param))>0;
        }
        /// <summary>
        /// 后台管理员列表
        /// </summary>
        /// <returns></returns>
        public static List<MemberInfo> AdminList() {
            List<MemberInfo> list = new List<MemberInfo>();
            string strSQL = "SELECT * FROM PagesAdmin WITH(NOLOCK)";
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(new MemberInfo() { 
                        Id = dr.Field<int>("UserId"),
                        UserName = dr.Field<string>("UserName"),
                        CreateDateTime = dr.Field<DateTime>("CreateDateTime")
                    });
                }
            }
            return list;
        }
        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="userId"></param>
        public static void DeleteAdmin(int userId) {
            string strSQL = "DELETE PagesAdmin WHERE UserId = @UserId";
            SqlParameter param = new SqlParameter("UserId",userId);
            SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,param);
        }
        #endregion
    }
}
