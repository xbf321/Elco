using System;

namespace Elco.Models
{
    public class MemberInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string RealName { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
