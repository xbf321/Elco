
using Elco.Models;
using Elco.Data;

namespace Elco.Services
{
    public static class ProductService
    {
        /// <summary>
        /// 添加或编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ProductInfo Create(ProductInfo model) {
            if (model.Id == 0)
            {
                model.Id = ProductManage.Insert(model);
            }
            else {
                ProductManage.Update(model);
            }
            return model;
        }
        /// <summary>
        /// 获得详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProductInfo Get(int id){
            return ProductManage.Get(id);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static IPageOfList<ProductInfo> List(ProductSearchSetting setting) {
            return ProductManage.List(setting);
        }
    }
}
