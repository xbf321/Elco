using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

using Elco.Models;
using Elco.Common;

namespace Elco.Data
{
    public static class FeedbackManage
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(FeedbackInfo model) {
            string strSQL = "INSERT INTO Feedback(Title,Content,[Address],Contact,Email,Phone,Fax,CreateDateTime,IsDeleted) VALUES(@Title,@Content,@Address,@Contact,@Email,@Phone,@Fax,GETDATE(),0);SELECT @@IDENTITY;";
            SqlParameter[] param = {
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("Address",SqlDbType.NVarChar),
                                    new SqlParameter("Contact",SqlDbType.NVarChar),
                                    new SqlParameter("Email",SqlDbType.NVarChar),
                                    new SqlParameter("Phone",SqlDbType.NVarChar),
                                    new SqlParameter("Fax",SqlDbType.NVarChar),
                                 };
            param[0].Value = model.Title ?? string.Empty;
            param[1].Value = model.Content ?? string.Empty;
            param[2].Value = model.Address ?? string.Empty;
            param[3].Value = model.Contact ?? string.Empty;
            param[4].Value = model.Email ?? string.Empty;
            param[5].Value = model.Phone ?? string.Empty;
            param[6].Value = model.Fax ?? string.Empty;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id){
            string strSQL = "UPDATE Feedback SET IsDeleted = 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);
        }

        /// <summary>
        /// 反馈列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<FeedbackInfo> List(SearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Feedback";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "CreateDateTime DESC";
            
            fp.Condition = " IsDeleted = 0";

            IList<FeedbackInfo> list = new List<FeedbackInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new FeedbackInfo(){
                        Id = dr.Field<int>("Id"),
                        Title = dr.Field<string>("Title"),
                        Content = dr.Field<string>("Content"),
                        Contact = dr.Field<string>("Contact"),
                        Email = dr.Field<string>("Email"),
                        Phone = dr.Field<string>("Phone"),
                        Address = dr.Field<string>("Address"),
                        Fax = dr.Field<string>("Fax"),
                        CreateDateTime = dr.Field<DateTime>("CreateDateTime")
                    });
                }
            }

            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL())); ;
            return new PageOfList<FeedbackInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
    }
}
