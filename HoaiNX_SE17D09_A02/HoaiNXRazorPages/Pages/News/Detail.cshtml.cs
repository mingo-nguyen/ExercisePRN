using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace HoaiNXRazorPages.Pages.News
{
    public class DetailModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly ILogger<DetailModel> _logger;

        public DetailModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ITagService tagService,
            ILogger<DetailModel> logger)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _logger = logger;
        }

        public NewsArticle? Article { get; set; }
        public List<NewsArticle> RelatedArticles { get; set; } = new();
        public List<TagWithCount> PopularTags { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToPage("/News/Index");
                }

                // Get the article
                Article = await _newsArticleService.GetNewsArticleByIdAsync(id);

                // Check if article exists and is published/active
                if (Article == null || !Article.NewsStatus)
                {
                    return NotFound();
                }

                // Load related articles
                await LoadRelatedArticlesAsync();

                // Load popular tags
                await LoadPopularTagsAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading article with ID: {ArticleId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the article.";
                return RedirectToPage("/News/Index");
            }
        }

        private async Task LoadRelatedArticlesAsync()
        {
            if (Article == null || !Article.CategoryId.HasValue)
            {
                return;
            }

            // Get all active articles
            var allArticles = await _newsArticleService.GetNewsArticlesAsync();
            var activeArticles = allArticles.Where(a => a.NewsStatus).ToList();

            // First try: Find articles in the same category
            var sameCategoryArticles = activeArticles
                .Where(a => a.CategoryId == Article.CategoryId &&
                           a.NewsArticleId != Article.NewsArticleId)
                .Take(3)
                .ToList();

            RelatedArticles.AddRange(sameCategoryArticles);

            // If we don't have enough related articles by category, try finding by tags
            if (RelatedArticles.Count < 3 && Article.Tags != null && Article.Tags.Any())
            {
                var articleTags = Article.Tags.Select(t => t.TagName?.ToLower()).ToList();

                var tagRelatedArticles = activeArticles
                    .Where(a => a.NewsArticleId != Article.NewsArticleId &&
                               !RelatedArticles.Contains(a) &&
                               a.Tags != null &&
                               a.Tags.Any(t => articleTags.Contains(t.TagName?.ToLower())))
                    .Take(3 - RelatedArticles.Count)
                    .ToList();

                RelatedArticles.AddRange(tagRelatedArticles);
            }

            // If we still need more, add recent articles
            if (RelatedArticles.Count < 3)
            {
                var recentArticles = activeArticles
                    .Where(a => a.NewsArticleId != Article.NewsArticleId &&
                               !RelatedArticles.Contains(a))
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(3 - RelatedArticles.Count)
                    .ToList();

                RelatedArticles.AddRange(recentArticles);
            }
        }

        private async Task LoadPopularTagsAsync()
        {
            // Get all active articles
            var allArticles = await _newsArticleService.GetNewsArticlesAsync();
            var activeArticles = allArticles.Where(a => a.NewsStatus).ToList();

            // Extract all tags and count occurrences
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

            // Convert the dictionary to a list of TagWithCount
            PopularTags = tagCounts
                .Select(tc => new TagWithCount { TagName = tc.Key, Count = tc.Value })
                .OrderByDescending(tc => tc.Count)
                .Take(15) // Limit to the 15 most popular tags
                .ToList();
        }

        public class TagWithCount
        {
            public string TagName { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }

    // View model class for tags with counts
    
}
