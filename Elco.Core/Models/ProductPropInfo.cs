
namespace Elco.Models
{
    /// <summary>
    /// 产品属性KEY表
    /// </summary>
    public class ProductPropInfo
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        #region == 扩展字段 ==
        /// <summary>
        /// 扩展字段用于产品显示用的Value
        /// </summary>
        public string Value { get; set; }
        #endregion

        public bool IsDeleted { get; set; }

        public ProductPropInfo() {
            Name = Value = string.Empty;
            IsDeleted = false;
        }
    }
}
