using System;

namespace Elco.Models
{
    /// <summary>
    /// 下载中心
    /// </summary>
    public class AttachmentInfo
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadUrl { get; set; }
        public bool IsDeleted { get; set; }
        public long Size { get; set; }
        public DateTime CreateDateTime { get; set; }
        public Guid GUID { get; set; }
        public int Sort { get; set; }
        
        public int DownloadCount { get; set; }
        public AttachmentInfo() {
            Size = 0;
            ImageUrl = DownloadUrl = "###";
            Sort = 999999;
        }
        /*
         CREATE TABLE Attachment(
	Id INT NOT NULL IDENTITY(1,1),
	Title NVARCHAR(255) NOT NULL DEFAULT(''),
	ImageUrl NVARCHAR(255) NOT NULL DEFAULT(''),
	DownloadUrl NVARCHAR(255) NOT NULL DEFAULT(''),
	CreateDateTime DATETIME NOT NULL DEFAULT(GETDATE()),
	PRIMARY KEY(Id)
)
         */
    }
}
