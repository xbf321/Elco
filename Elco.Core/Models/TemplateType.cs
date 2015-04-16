using System.ComponentModel;

namespace Elco.Models
{
    public enum TemplateType
    {
        //[Description("默认（类别描述）")]
        [Description("Default（description）")]
        None = 0,
        //[Description("文章列表")]
        [Description("Article list")]
        ArticleList = 1,
        //[Description("图片文章列表")]
        [Description("Pic+Article")]
        ArticleListWithImage = 2
    }
}
