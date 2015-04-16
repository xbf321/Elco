
namespace Elco.Models
{
    public class PageOptions
    {
        public PageOptions() {
            PrevText = "上一页";
            NextText = "下一页";
            LastText = "尾页";
            FirstText = "首页";
            TotalText = "共";
        }
        public string PrevText { get; set; }
        public string NextText { get; set; }
        public string LastText { get; set; }
        public string FirstText { get; set; }
        public string TotalText { get; set; }
    }
}
