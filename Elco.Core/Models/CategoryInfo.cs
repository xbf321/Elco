using System;
using System.Collections.Generic;

namespace Elco.Models
{
    public class CategoryInfo : IEquatable<CategoryInfo>
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string ParentIdList { get; set; } 
        /// <summary>
        /// 类别名称
        /// </summary>       
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 页面的Banner广告图片链接
        /// 目前只针对父节点，子节点字段没有作用
        /// </summary>
        public string BannerAdImageUrl { get; set; }
        /// <summary>
        /// 快速链接
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 类别描述
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 模板类型[来自Hite.Model.TemplateType]
        /// </summary>
        public int TemplateType { get; set; }
        
        /// <summary>
        /// 是否显示第一个节点的内容
        /// 相当于跳到第一个节点，但是Url不变
        /// </summary>
        public bool IsShowFirstChildNode { get; set; }
        /// <summary>
        /// 是否启用，主要针对有的类别前台不显示，但是要读取类别中的内容，
        /// 比如首页中的焦点图，这样就不用新建焦点图的表了，直接用文章的就行了
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public WebLanguage Language { get; set; }

        #region == Extension Protype
        /// <summary>
        /// 扩展属性，自动填充
        /// </summary>
        public IList<ArticleInfo> ArticleList { get; set; }
        /// <summary>
        /// 扩展属性，自动填充
        /// </summary>
        public string Url { get; set; }
        #endregion

        public CategoryInfo() {
            IsEnabled = true;
            IsDeleted = false;
            IsShowFirstChildNode = false;
            BannerAdImageUrl = ImageUrl = "###";
            Sort = 999999;
        }

        #region IEquatable<CategoryInfo> Members

        public bool Equals(CategoryInfo other)
        {
            if (other == null) return false;
            if (this == null) return false;
            return this.Id.Equals(other.Id);
        }
        public override int GetHashCode()
        {

            //Get hash code for the Code field.
            int hashProductCode = this.Id.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductCode;
        }

        #endregion
    }
}
