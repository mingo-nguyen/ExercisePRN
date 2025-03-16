using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HoaiNXRazorPages.Pages.Staff
{
    [Authorize(Policy = "RequireStaffRole")]
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ILogger<IndexModel> logger)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public int TotalArticles { get; private set; }
        public int ActiveArticlesCount { get; private set; }
        public int DraftArticlesCount { get; private set; }
        public int MyArticlesCount { get; private set; }
        public int CategoriesCount { get; private set; }
        public List<NewsArticle> RecentArticles { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get current user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                short? accountId = null;

                if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out short parsedId))
                {
                    accountId = parsedId;
                }

                // Get all articles
                var allArticles = await _newsArticleService.GetNewsArticlesAsync();

                // Calculate statistics
                TotalArticles = allArticles.Count();
                ActiveArticlesCount = allArticles.Count(a => a.NewsStatus);
                DraftArticlesCount = allArticles.Count(a => !a.NewsStatus);

                if (accountId.HasValue)
                {
                    MyArticlesCount = allArticles.Count(a => a.CreatedById == accountId);
                }

                // Get recent articles (5 most recent)
                RecentArticles = allArticles
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(5)
                    .ToList();

                // Get categories count
                var categories = await _categoryService.GetCategoriesAsync();
                CategoriesCount = categories.Count();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff dashboard");
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return Page();
            }
        }
    }
}
