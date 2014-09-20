using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Elco.Models;
using Elco.Common;

namespace Elco.Data
{
    internal static class ArticleManage
    {

        #region == Add ==
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ArticleInfo model) {
            string strSQL = "INSERT INTO Articles(CategoryId,Title,Content,ImageUrl,LinkUrl,Sort,IsTop,IsDeleted,PublishDateTime,CreateDateTime,Remark,Timespan,ViewCount,IsPublished,FullTimespan,Keywords,SubTitle) VALUES(@CategoryId,@Title,@Content,@ImageUrl,@LinkUrl,@Sort,@IsTop,@IsDeleted,@PublishDateTime,GETDATE(),@Remark,@Timespan,1,@IsPublished,@FullTimespan,@Keywords,@SubTitle);SELECT @@IDENTITY;";


            //为了保证Timespan和FullTimespan一致
            string timespan = DateTime.Now.ToString("HHmmssfff");

            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("LinkUrl",SqlDbType.NVarChar),
                                    new SqlParameter("IsTop",SqlDbType.Int),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("Remark",SqlDbType.NVarChar),
                                    new SqlParameter("TimeSpan",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("IsPublished",SqlDbType.Int),
                                    new SqlParameter("FullTimespan",SqlDbType.NVarChar),
                                    new SqlParameter("Keywords",SqlDbType.NVarChar),
                                    new SqlParameter("SubTitle",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty :model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.ImageUrl) ? string.Empty : model.ImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.LinkUrl) ? string.Empty : model.LinkUrl;
            parms[6].Value = model.IsTop;
            parms[7].Value = model.IsDeleted;            
            parms[8].Value = model.PublishDateTime<= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[9].Value = string.IsNullOrEmpty(model.Remark) ? string.Empty : model.Remark;
            parms[10].Value = timespan;
            parms[11].Value = model.Sort;
            parms[12].Value = model.IsPublished ? 1 : 0;
            parms[13].Value = string.Format("{0}{1}",DateTime.Now.ToString("yyyyMMdd"),timespan);
            parms[14].Value = model.Keywords ?? string.Empty;
            parms[15].Value = model.SubTitle ?? string.Empty;
            return Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        #endregion

        #region == Update ==
        /// <summary>
       /// 更新
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        public static int Update(ArticleInfo model) {
            string strSQL = "UPDATE Articles SET CategoryId = @CategoryId,Title = @Title,Content = @Content,ImageUrl = @ImageUrl,LinkUrl = @LinkUrl,Sort = @Sort,IsTop = @IsTop,PublishDateTime = @PublishDateTime,IsDeleted = @IsDeleted,Remark = @Remark,IsPublished = @IsPublished,Keywords = @Keywords,SubTitle = @SubTitle WHERE ID = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Content",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("LinkUrl",SqlDbType.NVarChar),
                                    new SqlParameter("IsTop",SqlDbType.Int),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("PublishDateTime",SqlDbType.DateTime),
                                    new SqlParameter("Remark",SqlDbType.NVarChar),
                                    new SqlParameter("TimeSpan",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("IsPublished",SqlDbType.Int),
                                    new SqlParameter("Keywords",SqlDbType.NVarChar),
                                    new SqlParameter("SubTitle",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = string.IsNullOrEmpty(model.Title) ? string.Empty : model.Title;
            parms[3].Value = string.IsNullOrEmpty(model.Content) ? string.Empty : model.Content;
            parms[4].Value = string.IsNullOrEmpty(model.ImageUrl) ? string.Empty : model.ImageUrl;
            parms[5].Value = string.IsNullOrEmpty(model.LinkUrl) ? string.Empty : model.LinkUrl;
            parms[6].Value = model.IsTop;
            parms[7].Value = model.IsDeleted;
            parms[8].Value = model.PublishDateTime <= DateTime.MinValue ? DateTime.Now : model.PublishDateTime;
            parms[9].Value = string.IsNullOrEmpty(model.Remark) ? string.Empty : model.Remark;
            parms[10].Value = string.IsNullOrEmpty(model.Timespan) ? DateTime.Now.ToString("HHmmssfff") : model.Timespan;
            parms[11].Value = model.Sort;
            parms[12].Value = model.IsPublished ? 1 : 0;
            parms[13].Value = model.Keywords ?? string.Empty;
            parms[14].Value = model.SubTitle ?? string.Empty;

            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        #endregion        

        #region == ListWithPage ==
        public static IPageOfList<ArticleInfo> List(ArticleSearchSetting setting)
        {
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("PublishDate",SqlDbType.NVarChar)
                                   };
            parms[0].Value = setting.CategoryId;
            parms[1].Value = setting.Title;
            parms[2].Value = setting.PublishDate;

            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Articles";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = "IsTop DESC,Sort ASC,PublishDateTime DESC";
            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append(@"EXISTS(
		                            SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                            WHERE (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                            AND p.CategoryId = AC.ID
                                )");
            if (!setting.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 /*获取未删除的*/");
            }
            if (!string.IsNullOrEmpty(setting.Title))
            {
                //虚拟服务器，没有启用全文索引
                //sbCondition.Append("    AND CONTAINS(Title,@Title)  ");

                sbCondition.Append("    AND Title LIKE '%'+@Title+'%'  ");

            }
            if (Regex.IsMatch(setting.PublishDate, @"^\d{4}-\d{1,2}-\d{1,2}$", RegexOptions.IgnoreCase))
            {
                sbCondition.Append("    AND CONVERT(VARCHAR(10),PublishDateTime,120) = @PublishDate");
            }
            if(setting.IsOnlyShowPublished){
                //只显示发布的文章
                sbCondition.Append("    AND IsPublished = 1 ");
            }

            fp.Condition = sbCondition.ToString();
            IList<ArticleInfo> list = new List<ArticleInfo>();
            ArticleInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(), parms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    if (model != null)
                    {
                        list.Add(model);
                    }
                }
            }

            int count = Convert.ToInt32(Goodspeed.Library.Data.SQLPlus.ExecuteScalar(CommandType.Text, fp.BuildCountSQL(), parms)); ;
            return new PageOfList<ArticleInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        #endregion

        #region == ListWithoutPage ==
        /// <summary>
        /// 获得文章列表
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="categoryId"></param>
        /// <param name="topCount">默认10条</param>
        /// <returns></returns>
        public static List<ArticleInfo> ListWithoutPage(int categoryId, int topCount, bool isTopOneImg = false)
        {
            List<ArticleInfo> list = new List<ArticleInfo>();
            if (categoryId == 0) { return list; }
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append(@"WITH TEP_Articles AS (
	                        SELECT TOP(50) ROW_NUMBER() OVER(ORDER BY IsTop DESC,Sort ASC,PublishDateTime DESC/*首先按置顶，再按排序，其次按发布时间*/) AS RowNumber,* FROM Articles WITH(NOLOCK) WHERE EXISTS(
                                    SELECT * FROM Categories AS AC WITH(NOLOCK)
                                    WHERE  (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)	/*获取此分类下所有的子分类*/
                                    AND IsDeleted = 0 /*获取未删除的*/
                                    /*AND IsEnabled = 1*/ /*可能某个栏目就是默认不显示*/
                                    AND Articles.CategoryId = AC.ID
	                        ) AND IsDeleted = 0 /*获取未删除的*/
	                        AND IsPublished = 1 /*获得发布的文章*/
)");
            if (isTopOneImg)
            {
                //第一张是图片
                sbSQL.Append(@",TEP_ImageArticles AS (
	                                SELECT TOP(1)* FROM TEP_Articles WHERE ImageUrl <> '' AND ImageUrl <> '###'
                                )
                                SELECT * FROM TEP_ImageArticles");
                //如果就取TOP(1),则不用再取剩余的
                if (topCount > 1)
                {
                    sbSQL.AppendFormat(@"   UNION ALL
                                            SELECT TOP({0})* FROM TEP_Articles AS P WHERE NOT EXISTS(
	                                            SELECT * FROM TEP_ImageArticles 
	                                            WHERE TEP_ImageArticles.RowNumber = P.RowNumber
                                            )", topCount - 1);
                }
                //throw new Exception(sbSQL.ToString());
            }
            else
            {
                sbSQL.AppendFormat("SELECT TOP({0})* FROM TEP_Articles", topCount);
            }
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                    new SqlParameter("TopCount",SqlDbType.Int),
                                   };
            parms[0].Value = categoryId;
            parms[1].Value = topCount <= 0 ? 10 : topCount;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, sbSQL.ToString(), parms);
            ArticleInfo model = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    model = Get(dr);
                    list.Add(model);
                }
            }
            return list;
        }
        #endregion

        #region == Update ViewCount ==
        /// <summary>
        /// 更新浏览数
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateViewCount(int id) {
            string strSQL = "UPDATE Articles SET ViewCount = ViewCount + 1 WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parm);
        }
        #endregion

        #region == 获得文章详细信息 ==
        public static ArticleInfo Get(int id) {
            if (id == 0) { return new ArticleInfo(); }
            string strSQL = "SELECT * FROM Articles WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm));
        }
        public static ArticleInfo GetByFullTimespan(string fullTimespan)
        {
            string strSQL = "SELECT TOP(1) * FROM Articles WITH(NOLOCK) WHERE FullTimespan = @fullTimespan";
            SqlParameter parm = new SqlParameter("FullTimespan", fullTimespan);

            return Get(Goodspeed.Library.Data.SQLPlus.ExecuteDataRow(CommandType.Text, strSQL, parm));
        }
        private static ArticleInfo Get(DataRow dr) {
            ArticleInfo model = new ArticleInfo();
            if(dr != null){
                model.CategoryId = dr.Field<int>("CategoryId");
                model.Content = dr.Field<string>("Content");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
                model.GUID = dr.Field<Guid>("GUID");
                model.Id = dr.Field<int>("Id");
                model.ImageUrl = dr.Field<string>("ImageUrl");
                model.IsDeleted = dr.Field<bool>("IsDeleted");
                model.IsTop = dr.Field<bool>("IsTop");
                model.LinkUrl = dr.Field<string>("LinkUrl");
                model.PublishDateTime = dr.Field<DateTime>("PublishDateTime");
                model.Remark = dr.Field<string>("Remark");
                model.Sort = dr.Field<int>("Sort");
                model.Timespan = dr.Field<string>("Timespan");
                model.Title = dr.Field<string>("Title");
                model.ViewCount = dr.Field<int>("ViewCount");
                model.IsPublished = dr.Field<bool>("IsPublished");
                model.FullTimespan = dr.Field<string>("FullTimespan");
                model.Keywords = dr.Field<string>("Keywords");
                model.SubTitle = dr.Field<string>("SubTitle");
            }
            return model;
        }
        #endregion

        #region == 设置文章发布状态 ==
        /// <summary>
        /// 设置文章发布状态
        /// </summary>
        /// <param name="status"></param>
        public static bool SetPublishStatus(int id,bool status) {
            string strSQL = string.Format("UPDATE Articles SET IsPublished = {0} WHERE Id = @Id",status?1:0);
            SqlParameter param = new SqlParameter("Id",id);
            return Goodspeed.Library.Data.SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, param) > 0;
        }
        #endregion

    }
}
