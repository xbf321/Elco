using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

using Elco.Models;
using Goodspeed.Library.Data;
using Elco.Common;


namespace Elco.Data
{
    public static class ProductManage
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ProductInfo model) {
            string strSQL = "INSERT INTO Products(Title,CategoryId,Introduction,ImageUrl,CategoryIdRoute,IsDeleted,SonModel,DocumentUrl,Sort) VALUES(@Title,@CategoryId,@Introduction,@ImageUrl,@CategoryIdRoute,@IsDeleted,@SonModel,@DocumentUrl,@Sort);SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Introduction",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("CategoryIdRoute",SqlDbType.NVarChar),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("SonModel",SqlDbType.NVarChar),
                                    new SqlParameter("DocumentUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.Title ?? string.Empty;
            parms[2].Value = model.Introduction ?? string.Empty;
            parms[3].Value = model.ImageUrl ?? string.Empty;
            parms[4].Value = model.CategoryId;
            parms[5].Value = model.CategoryIdRoute ?? string.Empty;
            parms[6].Value = model.IsDeleted ? 1 : 0;
            parms[7].Value = model.SonModel ?? string.Empty;
            parms[8].Value = model.DocumentUrl ?? string.Empty;
            parms[9].Value = model.Sort;

            int id = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
            model.Id = id;

            //更新属性信息
            UpdateProduct2PropValues(model);

            return id;
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(ProductInfo model) {
            string strSQL = "UPDATE Products SET ImageUrl = @ImageUrl ,Introduction = @Introduction, Title = @Title,IsDeleted = @IsDeleted,CategoryIdRoute = @CategoryIdRoute,SonModel = @SonModel,DocumentUrl = @DocumentUrl,Sort = @Sort,CategoryId = @CategoryId WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Title",SqlDbType.NVarChar),
                                    new SqlParameter("Introduction",SqlDbType.NVarChar),
                                    new SqlParameter("ImageUrl",SqlDbType.NVarChar),
                                    new SqlParameter("IsDeleted",SqlDbType.Int),
                                    new SqlParameter("CategoryIdRoute",SqlDbType.NVarChar),
                                    new SqlParameter("SonModel",SqlDbType.NVarChar),
                                    new SqlParameter("DocumentUrl",SqlDbType.NVarChar),
                                    new SqlParameter("Sort",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.Title ?? string.Empty;
            parms[2].Value = model.Introduction ?? string.Empty;
            parms[3].Value = model.ImageUrl ?? string.Empty;
            parms[4].Value = model.IsDeleted ? 1 : 0;
            parms[5].Value = model.CategoryIdRoute ?? string.Empty;
            parms[6].Value = model.SonModel ?? string.Empty;
            parms[7].Value = model.DocumentUrl ?? string.Empty;
            parms[8].Value = model.Sort;
            parms[9].Value = model.CategoryId;

            int i = SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms) ;

            if (i > 0)
            {
                //更新属性信息
                UpdateProduct2PropValues(model);

            }

            return i;
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProductInfo Get(int id) {
            string strSQL = "SELECT * FROM Products WITH(NOLOCK) WHERE Id = @Id";
            SqlParameter parm = new SqlParameter("Id",id);
            DataRow dr = SQLPlus.ExecuteDataRow(CommandType.Text,strSQL,parm);
            return GetByRow(dr);
        }
        /// <summary>
        /// 产品列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ProductInfo> List(ProductSearchSetting setting) {
            FastPaging fp = new FastPaging();
            fp.PageIndex = setting.PageIndex;
            fp.PageSize = setting.PageSize;
            fp.Ascending = false;
            fp.TableName = "Products";
            fp.TableReName = "p";
            fp.PrimaryKey = "ID";
            fp.QueryFields = "p.*";
            fp.OverOrderBy = " Sort ASC,CreateDateTime ASC";
            StringBuilder sbCondition = new StringBuilder();
            sbCondition.Append("    1 = 1 ");
            if (setting.CategoryId > 0)
            {
                sbCondition.Append(@"   AND EXISTS(
		                            SELECT * FROM Categories AS AC WITH(NOLOCK) 
		                            WHERE (AC.ID = @CID OR CHARINDEX(','+CAST(@CID AS NVARCHAR(MAX))+',',','+AC.ParentIdList+',') >0)
		                            AND p.CategoryId = AC.ID
                                )");
            }
            if (!setting.ShowDeleted)
            {
                sbCondition.Append("    AND IsDeleted = 0 /*获取未删除的*/");
            }
            SqlParameter[] parms = { 
                                    new SqlParameter("CID",SqlDbType.Int),
                                   };
            parms[0].Value = setting.CategoryId;
            

            fp.Condition = sbCondition.ToString();
            IList<ProductInfo> list = new List<ProductInfo>();
            ProductInfo model = null;
            DataTable dt = Goodspeed.Library.Data.SQLPlus.ExecuteDataTable(CommandType.Text, fp.Build2005(),parms);
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
            int count = Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,fp.BuildCountSQL(),parms));
            return new PageOfList<ProductInfo>(list, setting.PageIndex, setting.PageSize, count);
        }
        private static ProductInfo GetByRow(DataRow dr) {
            var model = new ProductInfo();
            if (dr != null) {
                model.CategoryId = dr.Field<int>("CategoryId");
                model.CreateDateTime = dr.Field<DateTime>("CreateDateTime");
                model.Id = dr.Field<int>("Id");
                model.ImageUrl = dr.Field<string>("ImageUrl");
                model.Introduction = dr.Field<string>("Introduction");
                model.Title = dr.Field<string>("Title");
                model.IsDeleted = dr.Field<bool>("IsDeleted");
                model.CategoryIdRoute = dr.Field<string>("CategoryIdRoute");
                model.SonModel = dr.Field<string>("SonModel");
                model.Props = GetProductProps(model.Id);
                model.DocumentUrl = dr.Field<string>("DocumentUrl");
                model.Sort = dr.Field<int>("Sort");
            }
            return model;            
        }
        #region == Product2PropValues ==
        /// <summary>
        /// 更新产品所使用的属性以及属性值
        /// </summary>
        /// <param name="model"></param>
        private static void UpdateProduct2PropValues(ProductInfo model)
        {
            //首先删除
            string strSQL = "DELETE dbo.Product2PropValues WHERE ProductId = @ProductId";
            SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, new SqlParameter("ProductId", SqlDbType.Int) { Value = model.Id });

            //在添加
            strSQL = "INSERT INTO dbo.Product2PropValues(ProductId,PropId,Value) VALUES(@ProductId,@PropId,@Value)";
            SqlParameter[] parms = { 
                                    new SqlParameter("ProductId",SqlDbType.Int),
                                    new SqlParameter("PropId",SqlDbType.Int),
                                    new SqlParameter("Value",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Id;
            foreach (var item in model.Props)
            {
                parms[1].Value = item.Id;
                if (!string.IsNullOrEmpty(item.Value))
                {
                    parms[2].Value = item.Value.Replace('"', '”'); 
                    SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
                }
            }

        }
        /// <summary>
        /// 获得产品属性
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private static List<ProductPropInfo> GetProductProps(int productId) {
            string strSQL = "SELECT P.Id,P.Name,V.Value FROM ProductProps AS P WITH(NOLOCK) INNER JOIN Product2PropValues AS V WITH(NOLOCK) ON P.Id = V.PropId WHERE P.IsDeleted = 0 AND V.ProductId = @ProductId";
            List<ProductPropInfo> list = new List<ProductPropInfo>();
            SqlParameter parm = new SqlParameter("ProductId",productId);
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(new ProductPropInfo() { 
                        Id = dr.Field<int>("Id"),
                        Name = dr.Field<string>("Name"),
                        Value = dr.Field<string>("Value")
                    });
                }
            }
            return list;
        }
        #endregion
    }
}
