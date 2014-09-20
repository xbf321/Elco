using System;

namespace Elco.Models
{
    /// <summary>
    /// 反馈信息
    /// </summary>
    public class FeedbackInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }
        public DateTime CreateDateTime { get; set; }
        /*
            CREATE TABLE Feedback(
	Id INT NOT NULL IDENTITY(1,1),
	Title NVARCHAR(255) NOT NULL DEFAULT(''),
	Content NVARCHAR(MAX) NOT NULL DEFAULT(''),
	Address NVARCHAR(255) NOT NULL DEFAULT(''),
	Contact NVARCHAR(20) NOT NULL DEFAULT(''),
	Email NVARCHAR(50) NOT NULL DEFAULT(''),
	Phone NVARCHAR(20) NOT NULL DEFAULT(''),
	Fax NVARCHAR(20) NOT NULL DEFAULT(''),
	CreateDateTime DATETIME NOT NULL DEFAULT(GETDATE()),
	PRIMARY KEY(Id)
)
         ALTER TABLE Feedback ADD IsDeleted BIT NOT NULL DEFAULT(0)
         */
    }
}
