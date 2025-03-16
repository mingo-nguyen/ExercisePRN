using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HoaiNXRazorPages.Pages.Staff
{
    public class NewsArticleFormModel
    {
        public NewsArticle NewsArticle { get; set; } = new NewsArticle();
        public string TagsInput { get; set; } = string.Empty;
        public SelectList CategorySelectList { get; set; }
        public bool IsPublished { get; set; } = false;
        public bool IsEdit { get; set; } = false;
    }
}
