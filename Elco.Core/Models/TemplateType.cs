using System.ComponentModel;

namespace Elco.Models
{
    public enum TemplateType
    {
        [Description("默认（类别描述）")]
        None = 0,
        [Description("文章列表")]
        ArticleList = 1,
        [Description("图片文章列表")]
        ArticleListWithImage = 2
    }
}
