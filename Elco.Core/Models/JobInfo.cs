using System;

namespace Elco.Models
{
    /// <summary>
    /// 招聘信息
    /// </summary>
    public class JobInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string WorkingPlace { get; set; }
        /// <summary>
        /// 招聘人数
        /// </summary>
        public int Number { get; set; }
        public bool IsPublished { get; set; }
        public int Sort { get; set; }
        public bool IsHot { get; set; }
        public DateTime CreateDateTime { get; set; }
        public JobInfo() {
            Sort = 999999;
            Number = 1;
        }
        /*
         CREATE TABLE Job(
	Id INT NOT NULL IDENTITY(1,1),
	Title NVARCHAR(255) NOT NULL DEFAULT(''),
	Introduction NVARCHAR(MAX) NOT NULL DEFAULT(''),
	WorkingPlace NVARCHAR(255) NOT NULL DEFAULT(''),
	IsPublished BIT NOT NULL DEFAULT(0),
	Sort INT NOT NULL DEFAULT(0),
	IsHot BIT NOT NULL DEFAULT(0),
	CreateDateTime DATETIME NOT NULL DEFAULT(GETDATE()),
	PRIMARY KEY(Id)
)
         * Id, Title, Introduction, WorkingPlace, IsPublished, Sort, IsHot, IsDeleted, CreateDateTime, Number
         */
    }
}
