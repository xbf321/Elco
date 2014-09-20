using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Elco.Common;
using Elco.Models;
using System.Text;


namespace Elco.Data
{
    public static class AttachmentManage
    {
        #region == Attachment ==
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(AttachmentInfo model) {
            string strSQL = "INSERT INTO dbo.Attachment(Title,ImageUrl,DownloadUrl,IsDeleted,CreateDateTime,Size,Sort,CategoryId) VALUES(@Title,@ImageUrl,@DownloadUrl,0,GETDATE(),@Size,@Sort,@CategoryId);SELECT @@IDENTITY;";
            SqlParameter[] param = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("DownloadUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Size",SqlDbType.Int),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                   };
            param[0].Value = model.Id;
            param[1].Value = model.Title ?? string.Empty;
            param[2].Value = model.ImageUrl ?? string.Empty;
            param[3].Value = model.DownloadUrl ?? string.Empty;
            param[4].Value = model.Size;
            param[5].Value = model.Sort;
            param[6].Value = model.CategoryId;

            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(AttachmentInfo model) {
            string strSQL = "UPDATE dbo.Attachment SET Title = @Title,ImageUrl = @ImageUrl,DownloadUrl = @DownloadUrl,Size = @Size ,Sort = @Sort,CategoryId = @CategoryId WHERE Id = @Id";
            SqlParameter[] param = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("DownloadUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Size",SqlDbType.Int),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                   };
            param[0].Value = model.Id;
            param[1].Value = model.Title ?? string.Empty;
            param[2].Value = model.ImageUrl ?? string.Empty;
            param[3].Value = model.DownloadUrl ?? string.Empty;
            param[4].Value = model.Size;
            param[5].Value = model.Sort;
            param[6].Value = model.CategoryId;

            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,param);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id) {
            string strSQL = "UPDATE  dbo.Attachment SET IsDeleted = 1 WHERE Id = @Id";
            SqlParameter param = new SqlParameter("Id",id);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, param);
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AttachmentInfo Get(int id) {
            string strSQL = "SELECT * FROM dbo.Attachment WHERE Id = @Id ";
            SqlParameter param = new SqlParameter("Id", id);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,param));
        }
        /// <summary>
        /// 根据GUID获得详细信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static AttachmentInfo GetByGUID(string guid) {
            string strSQL = "SELECT * FROM dbo.Attachment WITH(NOLOCK) WHERE GUID = @GUID ";
            SqlParameter param = new SqlParameter("GUID", guid);
            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, param));
        }
        /// <summary>
        /// 更新下载次数
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateDownloadCount(int id) {
            string strSQL = "UPDATE Attachment SET DownloadCount = DownloadCount + 1 WHERE Id= @Id";
            SqlParameter param = new SqlParameter("Id", id);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,param);
        }
        private static AttachmentInfo Get(DataRow dr) {
            if (dr == null) { return new AttachmentInfo(); }
            return new AttachmentInfo() {
                Id = dr.Field<int>("Id"),
                Title = dr.Field<string>("Title"),
                ImageUrl = dr.Field<string>("ImageUrl"),
                DownloadUrl = dr.Field<string>("DownloadUrl"),
                CreateDateTime = dr.Field<DateTime>("CreateDateTime"),
                Size = dr.Field<long>("Size"),
                Sort = dr.Field<int>("Sort"),
                GUID = dr.Field<Guid>("GUID"),
                DownloadCount = dr.Field<int>("DownloadCount"),
                CategoryId = dr.Field<int>("CategoryId")
            };
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<AttachmentInfo> List(AttachmentSearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Attachment";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " Sort ASC,CreateDateTime DESC";
            fp.WithOptions = " WITH(NOLOCK)";

            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append("    IsDeleted = 0 ");
            if (setting.CategoryId > 0)
            {
                sbCondition.Append(@"   AND EXISTS(
		                            SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                            WHERE (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                            AND p.CategoryId = AC.ID
                                )");
            }
            fp.Condition = sbCondition.ToString();

            SqlParameter[] param = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                   };
            param[0].Value = setting.CategoryId;

            IList<AttachmentInfo> list = new List<AttachmentInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(),param);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(Get(dr));
                }
            }

            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL(),param)); ;
            return new PageOfList<AttachmentInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        #endregion

        #region == AttachmentDownloadLog ==
        /// <summary>
        /// 插入下载日志
        /// </summary>
        /// <param name="model"></param>
        public static void InsertLog(AttachmentDownloadLogInfo model) {
            string strSQL = @"DECLARE @C AS INT;
SELECT @C = COUNT(*) FROM AttachmentDownloadLog WITH(NOLOCK) WHERE AttachId = @AttachId AND UserId = @UserId
IF(@C =0)
	BEGIN
		INSERT INTO AttachmentDownloadLog(AttachId,UserId,UserName,DownloadCount,LastDownloadDateTime) VALUES(@AttachId,@UserId,@UserName,1,GETDATE());
	END
ElSE
	BEGIN
		UPDATE AttachmentDownloadLog SET DownloadCount = DownloadCount+1 ,LastDownloadDateTime = GETDATE() WHERE AttachId = @AttachId AND UserId = @UserId;
	END";
            SqlParameter[] param = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("AttachId",SqlDbType.Int),
                                    new SqlParameter("UserId",SqlDbType.Int),
                                    new SqlParameter("UserName",SqlDbType.NVarChar)
                                   };
            param[0].Value = model.Id;
            param[1].Value = model.AttachId;
            param[2].Value = model.UserId;
            param[3].Value = model.UserName;
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,param);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<AttachmentDownloadLogInfo> ListLog(int attachId,SearchSetting setting)
        {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "AttachmentDownloadLog";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*,A.Title";
            fp.OverOrderBy = " LastDownloadDateTime DESC";
            fp.WithOptions = " WITH(NOLOCK)";
            fp.JoinSQL = " INNER JOIN Attachment AS A WITH(NOLOCK) ON A.Id = P.AttachId";

            if(attachId>0){
                fp.Condition = string.Format("    AttachId = {0}",attachId);
            }
            IList<AttachmentDownloadLogInfo> list = new List<AttachmentDownloadLogInfo>();
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new AttachmentDownloadLogInfo() { 
                        Id = dr.Field<int>("Id"),
                        UserId = dr.Field<int>("UserId"),
                        UserName = dr.Field<string>("UserName"),
                        DownloadCount = dr.Field<int>("DownloadCount"),
                        LastDownloadDateTime = dr.Field<DateTime>("LastDownloadDateTime"),
                        AttachTitle = dr.Field<string>("Title")
                    });
                }
            }

            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL())); ;
            return new PageOfList<AttachmentDownloadLogInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        #endregion
    }
}
