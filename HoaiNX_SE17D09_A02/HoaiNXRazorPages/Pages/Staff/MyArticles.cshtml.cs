using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HoaiNXRazorPages.Pages.Staff
{
    [Authorize(Policy = "RequireStaffRole")]
    public class MyArticlesModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<MyArticlesModel> _logger;

        public MyArticlesModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ILogger<MyArticlesModel> logger)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public IEnumerable<NewsArticle> MyNewsArticles { get; set; } = new List<NewsArticle>();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "newest";

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short accountId))
                {
                    _logger.LogWarning("Failed to get user ID from claims");
                    TempData["ErrorMessage"] = "Cannot identify the current user.";
                    return Page();
                }

                // Get all articles
                var allArticles = await _newsArticleService.GetNewsArticlesAsync();

                // Filter by current user
                var myArticles = allArticles.Where(a => a.CreatedById == accountId).ToList();

                // Apply status filter
                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    bool status = StatusFilter.ToLower() == "active";
                    myArticles = myArticles.Where(a => a.NewsStatus == status).ToList();
                }

                // Apply search term
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    string search = SearchTerm.ToLower();
                    myArticles = myArticles.Where(a =>
                        (a.NewsTitle?.ToLower().Contains(search) ?? false) ||
                        (a.Headline?.ToLower().Contains(search) ?? false) ||
                        (a.NewsContent?.ToLower().Contains(search) ?? false) ||
                        (a.Category?.CategoryName?.ToLower().Contains(search) ?? false)
                    ).ToList();
                }

                // Apply sorting
                switch (SortBy?.ToLower())
                {
                    case "oldest":
                        myArticles = myArticles.OrderBy(a => a.CreatedDate).ToList();
                        break;
                    case "title":
                        myArticles = myArticles.OrderBy(a => a.NewsTitle).ToList();
                        break;
                    case "newest":
                    default:
                        myArticles = myArticles.OrderByDescending(a => a.CreatedDate).ToList();
                        break;
                }

                MyNewsArticles = myArticles;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff member's articles");
                TempData["ErrorMessage"] = "An error occurred while loading your articles.";
                MyNewsArticles = Enumerable.Empty<NewsArticle>();
                return Page();
            }
        }

        // Add a convenience method to determine if an article can be published
        public bool CanPublish(NewsArticle article)
        {
            return !article.NewsStatus;
        }

        // Add a convenience method to determine if an article can be unpublished
        public bool CanUnpublish(NewsArticle article)
        {
            return article.NewsStatus;
        }

        // Add a helper method to get article status as a friendly string
        public string GetArticleStatus(bool status)
        {
            return status ? "Active" : "Draft";
        }

        // Add a helper method to format dates nicely
        public string FormatDate(DateTime? date)
        {
            return date?.ToString("MMM dd, yyyy") ?? "N/A";
        }

        // Add a helper method to truncate text
        public string TruncateText(string text, int maxLength = 100)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text ?? string.Empty;

            return text.Substring(0, maxLength) + "...";
        }
    }
}
