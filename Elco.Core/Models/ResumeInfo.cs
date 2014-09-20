using System;

namespace Elco.Models
{
    /// <summary>
    /// 简历
    /// </summary>
    public class ResumeInfo
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Realname { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string GUID { get; set; }
        /// <summary>
        /// 工作经历
        /// </summary>
        public string WorkExperience { get; set; }
        /// <summary>
        /// 教育信息
        /// </summary>
        public string Education { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string Degree { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 家庭成员
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string MaritalStatus { get; set; }
        /// <summary>
        /// 出生地
        /// </summary>
        public string BirthPlace { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ResumeInfo() {
            GUID = Guid.NewGuid().ToString();
        }
    }
}
