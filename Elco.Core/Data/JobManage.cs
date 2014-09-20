using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Elco.Models;
using Elco.Common;


namespace Elco.Data
{
    public static class JobManage
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(JobInfo model) {
            string strSQL = "INSERT INTO Job(Title,Introduction,WorkingPlace,IsPublished,Sort,IsHot,IsDeleted,CreateDateTime,Number) VALUES(@Title,@Introduction,@WorkingPlace,@IsPublished,@Sort,@IsHot,0,GETDATE(),@Number);SELECT @@IDENTITY;";
            SqlParameter[] param = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Introduction",SqlDbType.NVarChar),
                                    new SqlParameter("WorkingPlace",SqlDbType.NVarChar),
                                    new SqlParameter("IsPublished",SqlDbType.Int),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("IsHot",SqlDbType.Int),
                                    new SqlParameter("Number",SqlDbType.Int),
                                   };
            param[0].Value = model.Id;
            param[1].Value = model.Title ?? string.Empty;
            param[2].Value = model.Introduction ?? string.Empty;
            param[3].Value = model.WorkingPlace ?? string.Empty;
            param[4].Value = model.IsPublished;
            param[5].Value = model.Sort;
            param[6].Value = model.IsHot;
            param[7].Value = model.Number;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(JobInfo model) {
            string strSQL = "UPDATE Job SET  Title = @Title ,Introduction = @Introduction,WorkingPlace = @WorkingPlace,IsPublished = @IsPublished ,Sort = @Sort,IsHot = @IsHot,Number = @Number WHERE Id = @Id";
            SqlParameter[] param = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Introduction",SqlDbType.NVarChar),
                                    new SqlParameter("WorkingPlace",SqlDbType.NVarChar),
                                    new SqlParameter("IsPublished",SqlDbType.Int),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("IsHot",SqlDbType.Int),
                                    new SqlParameter("Number",SqlDbType.Int),
                                   };
            param[0].Value = model.Id;
            param[1].Value = model.Title ?? string.Empty;
            param[2].Value = model.Introduction ?? string.Empty;
            param[3].Value = model.WorkingPlace ?? string.Empty;
            param[4].Value = model.IsPublished;
            param[5].Value = model.Sort;
            param[6].Value = model.IsHot;
            param[7].Value = model.Number;
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, param);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<JobInfo> List(JobSearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Job";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " IsHot DESC,Sort DESC ,CreateDateTime DESC";
            fp.WithOptions = " WITH(NOLOCK)";

            fp.Condition = " IsDeleted = 0";
            if(setting.IsOnlyShowPublished){
                fp.Condition += "   AND IsPublished = 1 ";
            }

            IList<JobInfo> list = new List<JobInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(GetByRow(dr));
                }
            }

            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL())); ;
            return new PageOfList<JobInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        /// <summary>
        /// 设置发布
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag">true:发布,false:不发布</param>
        /// <returns></returns>
        public static int SetPublish(int id,bool flag) {
            string strSQL = string.Format("UPDATE Job SET IsPublished = {0} WHERE Id = @Id",flag ? 1 : 0);
            SqlParameter param = new SqlParameter("Id", id);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, param);
        }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            string strSQL = "UPDATE Job SET IsDeleted = 1 WHERE Id = @Id";
            SqlParameter param = new SqlParameter("Id", id);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, param);
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JobInfo Get(int id) {
            string strSQL = "SELECT * FROM Job WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter param = new SqlParameter("Id",id);
            DataRow dr = Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,param);
            return GetByRow(dr);
        }
        private static JobInfo GetByRow(DataRow dr) {
            if (dr == null) { return new JobInfo(); }
            return new JobInfo() {
                Id = dr.Field<int>("Id"),
                Title = dr.Field<string>("Title"),
                Introduction = dr.Field<string>("Introduction"),
                IsHot = dr.Field<bool>("IsHot"),
                IsPublished = dr.Field<bool>("IsPublished"),
                Sort = dr.Field<int>("Sort"),
                WorkingPlace = dr.Field<string>("WorkingPlace"),
                CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                Number = dr.Field<int>("Number")
            };
        }
    }
}
