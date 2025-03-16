using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace HoaiNXRazorPages.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly ILogger<IndexModel> _logger;
        private const int PageSize = 6; // Articles per page

        public IndexModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ITagService tagService,
            ILogger<IndexModel> logger)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _logger = logger;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IEnumerable<CategoryWithCount> Categories { get; set; } = new List<CategoryWithCount>();
        public IEnumerable<TagWithCount> PopularTags { get; set; } = new List<TagWithCount>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public short? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TagSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "newest";

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }
        public int TotalArticles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get all active articles
                var allArticles = await _newsArticleService.GetNewsArticlesAsync();
                var activeArticles = allArticles.Where(a => a.NewsStatus).ToList();

                // Apply filters
                var filteredArticles = FilterArticles(activeArticles);

                // Sort articles
                filteredArticles = SortArticles(filteredArticles);

                // Get total count for pagination
                TotalArticles = filteredArticles.Count();
                TotalPages = (int)Math.Ceiling(TotalArticles / (double)PageSize);

                // Adjust current page if out of range
                if (CurrentPage < 1) CurrentPage = 1;
                if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

                // Apply pagination
                NewsArticles = filteredArticles
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                // Load categories with article count
                await LoadCategoriesWithCount(activeArticles);

                // Load popular tags
                await LoadPopularTags(activeArticles);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news articles index");
                TempData["ErrorMessage"] = "An error occurred while loading news articles.";
                NewsArticles = Enumerable.Empty<NewsArticle>();
                Categories = Enumerable.Empty<CategoryWithCount>();
                PopularTags = Enumerable.Empty<TagWithCount>();
                return Page();
            }
        }

        private List<NewsArticle> FilterArticles(List<NewsArticle> articles)
        {
            var filtered = articles;

            // Filter by search term
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                string searchLower = SearchTerm.ToLower();
                filtered = filtered.Where(a =>
                    (a.NewsTitle?.ToLower().Contains(searchLower) ?? false) ||
                    (a.Headline?.ToLower().Contains(searchLower) ?? false) ||
                    (a.NewsContent?.ToLower().Contains(searchLower) ?? false)
                ).ToList();
            }

            // Filter by category
            if (CategoryId.HasValue)
            {
                filtered = filtered.Where(a => a.CategoryId == CategoryId).ToList();
            }

            // Filter by tag
            if (!string.IsNullOrEmpty(TagSearch))
            {
                string tagLower = TagSearch.ToLower();
                filtered = filtered.Where(a =>
                    a.Tags != null && a.Tags.Any(t =>
                        t.TagName?.ToLower() == tagLower
                    )
                ).ToList();
            }

            // Filter by date range
            if (StartDate.HasValue)
            {
                filtered = filtered.Where(a => a.CreatedDate >= StartDate).ToList();
            }

            if (EndDate.HasValue)
            {
                // Add one day to include the end date fully
                var endDatePlusOne = EndDate.Value.AddDays(1);
                filtered = filtered.Where(a => a.CreatedDate < endDatePlusOne).ToList();
            }

            return filtered;
        }

        private List<NewsArticle> SortArticles(List<NewsArticle> articles)
        {
            return SortOrder?.ToLower() switch
            {
                "oldest" => articles.OrderBy(a => a.CreatedDate).ToList(),
                "title" => articles.OrderBy(a => a.NewsTitle).ToList(),
                _ => articles.OrderByDescending(a => a.CreatedDate).ToList() // "newest" or default
            };
        }

        private async Task LoadCategoriesWithCount(List<NewsArticle> activeArticles)
        {
            var categories = await _categoryService.GetCategoriesAsync();

            Categories = categories
                .Select(c => new CategoryWithCount
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName ?? "Uncategorized",
                    ArticleCount = activeArticles.Count(a => a.CategoryId == c.CategoryId)
                })
                .Where(c => c.ArticleCount > 0) // Only show categories with articles
                .OrderByDescending(c => c.ArticleCount)
                .ToList();
        }

        private async Task LoadPopularTags(List<NewsArticle> activeArticles)
        {
            // Extract all tags from active articles
            var tagCounts = new Dictionary<string, int>();

            foreach (var article in activeArticles)
            {
                if (article.Tags == null) continue;

                foreach (var tag in article.Tags)
                {
                    if (string.IsNullOrEmpty(tag.TagName)) continue;

                    if (tagCounts.ContainsKey(tag.TagName))
                    {
                        tagCounts[tag.TagName]++;
                    }
                    else
                    {
                        tagCounts[tag.TagName] = 1;
                    }
                }
            }

            // Convert to list and order by count
            PopularTags = tagCounts
                .Select(t => new TagWithCount
                {
                    TagName = t.Key,
                    Count = t.Value
                })
                .OrderByDescending(t => t.Count)
                .Take(15) // Take top 15 tags
                .ToList();
        }

        // Helper method to generate pagination URLs
        public string GetPaginationUrl(int page)
        {
            if (page < 1) page = 1;
            if (page > TotalPages) page = TotalPages;

            var queryParams = new Dictionary<string, string>();

            // Add current query parameters
            if (!string.IsNullOrEmpty(SearchTerm)) queryParams["searchTerm"] = SearchTerm;
            if (CategoryId.HasValue) queryParams["categoryId"] = CategoryId.Value.ToString();
            if (!string.IsNullOrEmpty(TagSearch)) queryParams["tagSearch"] = TagSearch;
            if (StartDate.HasValue) queryParams["startDate"] = StartDate.Value.ToString("yyyy-MM-dd");
            if (EndDate.HasValue) queryParams["endDate"] = EndDate.Value.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(SortOrder)) queryParams["sortOrder"] = SortOrder;

            // Always add page parameter
            queryParams["currentPage"] = page.ToString();

            // Build query string
            StringBuilder sb = new StringBuilder("/News?");
            bool first = true;

            foreach (var param in queryParams)
            {
                if (!first) sb.Append('&');
                sb.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}");
                first = false;
            }

            return sb.ToString();
        }
    }

    // Helper classes for view models
    public class CategoryWithCount
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ArticleCount { get; set; }
    }

    public class TagWithCount
    {
        public string TagName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
