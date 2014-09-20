
namespace Elco.Models
{
    public class SearchSetting
    {
       
        /// <summary>
        /// 必填
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 必填，默认10
        /// </summary>
        public int PageSize { get; set; }      

        /// <summary>
        /// 是否显示删除的信息
        /// 默认False
        /// </summary>
        public bool ShowDeleted { get; set; }

        public SearchSetting()
        {
            PageIndex = 1;
            PageSize = 10;
            ShowDeleted = false;
        } 
    }
    public class ProductSearchSetting : SearchSetting {
        /// <summary>
        /// 类别
        /// </summary>
        public int CategoryId { get; set; }
    }
    public class JobSearchSetting : SearchSetting {
        /// <summary>
        /// 是否显示发布
        /// </summary>
        public bool IsOnlyShowPublished { get; set; }
    }
    public class ArticleSearchSetting : SearchSetting {
        /// <summary>
        /// 发布时间
        /// 格式2010-02-13
        /// </summary>
        public string PublishDate { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 是否显示发布的文章，默认false，发布的和不发布的都显示
        /// </summary>
        public bool IsOnlyShowPublished { get; set; }
        public ArticleSearchSetting() {
            Title = PublishDate = string.Empty;
        }
    }
    /// <summary>
    /// 下载
    /// </summary>
    public class AttachmentSearchSetting : SearchSetting {
        public int CategoryId { get; set; }
    }
}
