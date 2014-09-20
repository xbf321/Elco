using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Elco.Models;
using System.Data;
using System.Data.SqlClient;
using Elco.Common;
using Goodspeed.Library.Data;

namespace Elco.Data
{
    public static class ResumeManage
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ResumeInfo model) {
            string strSQL = "INSERT INTO Resumes(Realname,Gender,Birthday,Email,[GUID],CreateDateTime,JobId,WorkExperience,Education,[Address],Phone,Family,Degree,Major,MaritalStatus,BirthPlace) VALUES(@RealName,@Gender,@Birthday,@Email,@GUID,GETDATE(),@JobId,@WorkExperience,@Education,@Address,@Phone,@Family,@Degree,@Major,@MaritalStatus,@BirthPlace);SELECT @@IDENTITY;";
            SqlParameter[] param = { 
                                    new SqlParameter("Realname",SqlDbType.NVarChar),
                                    new SqlParameter("Gender",SqlDbType.NVarChar),
                                    new SqlParameter("Birthday",SqlDbType.NVarChar),
                                    new SqlParameter("Email",SqlDbType.NVarChar),
                                    new SqlParameter("GUID",SqlDbType.NVarChar),
                                    new SqlParameter("JobId",SqlDbType.Int),
                                    new SqlParameter("WorkExperience",SqlDbType.NVarChar),
                                    new SqlParameter("Education",SqlDbType.NVarChar),
                                    new SqlParameter("Address",SqlDbType.NVarChar),
                                    new SqlParameter("Phone",SqlDbType.NVarChar),
                                    new SqlParameter("Family",SqlDbType.NVarChar),
                                    new SqlParameter("Degree",SqlDbType.NVarChar),
                                    new SqlParameter("Major",SqlDbType.NVarChar),
                                    new SqlParameter("MaritalStatus",SqlDbType.NVarChar),
                                    new SqlParameter("BirthPlace",SqlDbType.NVarChar),
                                };
            param[0].Value = model.Realname ?? string.Empty;
            param[1].Value = model.Gender ?? "男";
            param[2].Value = model.Birthday ?? "1986-01-01";
            param[3].Value = model.Email ?? string.Empty;
            param[4].Value = model.GUID ?? Guid.NewGuid().ToString();
            param[5].Value = model.JobId;
            param[6].Value = model.WorkExperience ?? string.Empty;
            param[7].Value = model.Education ?? string.Empty;
            param[8].Value = model.Address ?? string.Empty;
            param[9].Value = model.Phone ?? string.Empty;
            param[10].Value = model.Family ?? string.Empty;
            param[11].Value = model.Degree ?? string.Empty;
            param[12].Value = model.Major ?? string.Empty;
            param[13].Value = model.MaritalStatus ?? string.Empty;
            param[14].Value = model.BirthPlace ?? string.Empty;
           return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 根据GUID获取 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ResumeInfo GetByGUID(string guid) {
            string strSQL = "SELECT * FROM Resumes WITH(NOLOCK) WHERE [GUID] = @GUID";
            SqlParameter parm = new SqlParameter("GUID",guid);
            return GetByRow(SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm));
        }
        /// <summary>
        /// 删除（逻辑删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            string strSQL = "UPDATE Resumes SET IsDeleted = 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("id", id);
            return SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);

        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ResumeInfo> List(SearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Resumes";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " CreateDateTime DESC";
            fp.WithOptions = " WITH(NOLOCK)";
            fp.Condition = " IsDeleted = 0";


            IList<ResumeInfo> list = new List<ResumeInfo>();
            ResumeInfo model = null;
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = GetByRow(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }
            int count = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL()));
            return new PageOfList<ResumeInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        private static ResumeInfo GetByRow(DataRow dr) {
            if (dr == null) { return new ResumeInfo(); }
            return new ResumeInfo() { 
                Id = dr.Field<int>("Id"),
                Realname = dr.Field<string>("Realname"),
                Birthday = dr.Field<string>("Birthday"),
                JobId = dr.Field<int>("JobId"),
                Email = dr.Field<string>("Email"),
                Gender = dr.Field<string>("Gender"),
                CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                Major = dr.Field<string>("Major"),
                Family = dr.Field<string>("Family"),
                Address = dr.Field<string>("Address"),
                WorkExperience = dr.Field<string>("WorkExperience"),
                Education = dr.Field<string>("Education"),
                Degree = dr.Field<string>("Degree"),
                Phone = dr.Field<string>("Phone"),
                MaritalStatus = dr.Field<string>("MaritalStatus"),
                BirthPlace = dr.Field<string>("BirthPlace")
            };
        }
    }
}
