using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class ProductPropService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProductPropInfo Create(ProductPropInfo model) {
            if (string.IsNullOrEmpty(model.Name)) { return model; }
            if (model.Id == 0)
            {
                model.Id = ProductPropManage.Insert(model);
            }
            else {
                ProductPropManage.Update(model);
            }
            return model;
        }
        /// <summary>
        /// 删除（逻辑删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(int propId,int cid)
        {
            return ProductPropManage.Delete(propId,cid);
        }
        /// <summary>
        /// 列出所有的属性名称
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<ProductPropInfo> List(int categoryId)
        {
            return ProductPropManage.List(categoryId);
        }
    }
}
