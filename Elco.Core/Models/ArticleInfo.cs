using System;

namespace Elco.Models
{
    public class ArticleInfo
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public int Sort { get; set; }
        public bool IsTop { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Timespan { get; set; }        
        public Guid GUID { get; set; }
        public int ViewCount { get; set; }
        public string Keywords { get; set; }
        public String SubTitle { get; set; }
        /// <summary>
        /// 时间戳的详细记录
        /// </summary>
        public string FullTimespan
        {
            get;
            set;
        }
        /// <summary>
        /// 是否发布
        /// 文章只有发布了，才显示
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// 扩展属性，自动填充
        /// </summary>
        public string Url { get; set; }

        public ArticleInfo() {
            ImageUrl = "###";
            Timespan = DateTime.Now.ToString("HHmmssfff");
            PublishDateTime = CreateDateTime = DateTime.Now;
            Sort = 999999;
            FullTimespan = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
        //2012-7-6 添加IsPublished
        //ALTER TABLE dbo.Articles ADD IsPublished BIT NOT NULL DEFAULT(0)
    }
}
