using System;
using System.Collections.Generic;

namespace Elco.Models
{
    public class ProductInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 当前类别的父路径如：0,8,12
        /// </summary>
        public string CategoryIdRoute { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int Sort { get; set; }
        /// <summary>
        /// 子型号
        /// </summary>
        public string SonModel { get; set; }
        /// <summary>
        /// 产品文档
        /// </summary>
        public string DocumentUrl { get; set; }

        #region == 扩展字段 ==
        /// <summary>
        /// 产品属性
        /// </summary>
        public List<ProductPropInfo> Props { get; set; }
        #endregion

        public ProductInfo() {
            this.Props = new List<ProductPropInfo>();
            Sort = 999999;
        }
    }
}
