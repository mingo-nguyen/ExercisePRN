
using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HoaiNXRazorPages.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class ReportsModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        public ReportsModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }


        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public bool ShowReport => StartDate.HasValue && EndDate.HasValue;

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

        
        public async Task OnGetAsync()
        {
            if (!StartDate.HasValue)
            {
                StartDate = DateTime.Now.AddMonths(-20);
            }
            if(!EndDate.HasValue)
            {
                EndDate = DateTime.Now;
            }

            NewsArticles = await _newsArticleService.GetNewsArticlesByDateRangeAsync(StartDate.Value, EndDate.Value);


        }

    }
}
