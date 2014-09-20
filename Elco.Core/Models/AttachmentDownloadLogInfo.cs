using System;

namespace Elco.Models
{
    public class AttachmentDownloadLogInfo
    {
        public int Id { get; set; }
        public int AttachId { get; set; }
        /// <summary>
        /// 扩展字段，数据库中没有
        /// </summary>
        public string AttachTitle { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int DownloadCount { get; set; }
        public DateTime LastDownloadDateTime { get; set; }
    }
}
