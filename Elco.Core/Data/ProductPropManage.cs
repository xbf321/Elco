using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Elco.Models;
using Goodspeed.Library.Data;

namespace Elco.Data
{
    public static class ProductPropManage
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Insert(ProductPropInfo model) {
            string strSQL = "INSERT INTO ProductProps(CategoryId,Name,IsDeleted) VALUES(@CategoryId,@Name,0);SELECT @@IDENTITY;";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                    new SqlParameter("Name",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.CategoryId;
            parms[2].Value = model.Name.Replace('"', '”'); ;
            return Convert.ToInt32(SQLPlus.ExecuteScalar(CommandType.Text,strSQL,parms));
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(ProductPropInfo model) {
            string strSQL = "UPDATE ProductProps SET Name = @Name WHERE Id = @Id";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("Name",SqlDbType.NVarChar),
                                   };
            parms[0].Value = model.Id;
            parms[1].Value = model.Name.Replace('"', '”'); ;
            return SQLPlus.ExecuteNonQuery(CommandType.Text,strSQL,parms);
        }
        /// <summary>
        /// 删除（逻辑删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int id,int cid) {
            string strSQL = "UPDATE ProductProps SET IsDeleted = 1 WHERE ID = @Id AND CategoryId = @CategoryId";
            SqlParameter[] parms = { 
                                    new SqlParameter("Id",SqlDbType.Int),
                                    new SqlParameter("CategoryId",SqlDbType.Int),
                                   };
            parms[0].Value = id;
            parms[1].Value = cid;
            return SQLPlus.ExecuteNonQuery(CommandType.Text, strSQL, parms);
        }
        /// <summary>
        /// 列出所有的属性名称
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<ProductPropInfo> List(int categoryId) {
            List<ProductPropInfo> list = new List<ProductPropInfo>();
            string strSQL = "SELECT * FROM ProductProps WITH(NOLOCK) WHERE CategoryId = @CategoryId AND IsDeleted = 0";
            SqlParameter parm = new SqlParameter("CategoryId",categoryId);
            DataTable dt = SQLPlus.ExecuteDataTable(CommandType.Text,strSQL,parm);
            if(dt != null && dt.Rows.Count>0){
                foreach(DataRow dr in dt.Rows){
                    list.Add(new ProductPropInfo() { 
                        Id = dr.Field<int>("Id"),
                        CategoryId = dr.Field<int>("CategoryId"),
                        Name =dr.Field<string>("Name"),
                        IsDeleted = dr.Field<bool>("IsDeleted")
                    });
                }
            }
            return list;
        }
    }
}
