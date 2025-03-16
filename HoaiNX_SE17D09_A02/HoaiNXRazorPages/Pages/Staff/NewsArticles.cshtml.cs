using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HoaiNXRazorPages.Pages.Staff
{
    [Authorize(Policy = "RequireStaffRole")]
    public class NewsArticlesModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IHubContext<NewsHub> _hubContext;

        public NewsArticlesModel(
            INewsArticleService newsArticleService,
            ICategoryService categoryService,
            ITagService tagService,
            IHubContext<NewsHub> hubContext)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _hubContext = hubContext;
        }

        public IEnumerable<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

        public async Task OnGetAsync()
        {
            NewsArticles = await _newsArticleService.GetNewsArticlesAsync();
        }

        public async Task<IActionResult> OnGetNewsArticleFormAsync(string newsId)
        {
            var model = new NewsArticleFormModel();

            // Populate categories for dropdown
            var categories = await _categoryService.GetCategoriesAsync();
            model.CategorySelectList = new SelectList(categories, nameof(Category.CategoryId), nameof(Category.CategoryName));

            // If newsId is provided, this is an edit operation
            if (!string.IsNullOrEmpty(newsId))
            {
                var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(newsId);
                if (newsArticle == null)
                {
                    return new JsonResult(new { success = false, message = "Article not found" });
                }

                model.NewsArticle = newsArticle;
                model.IsEdit = true;
                model.IsPublished = newsArticle.NewsStatus;

                // Convert tags to comma-separated string
                if (newsArticle.Tags != null && newsArticle.Tags.Any())
                {
                    model.TagsInput = string.Join(", ", newsArticle.Tags.Select(t => t.TagName));
                }
            }

            return Partial("_NewsArticleFormPartial", model);
        }

        public async Task<IActionResult> OnPostCreateNewsArticleAsync([FromForm] NewsArticleFormModel formModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short accountId))
                {
                    return new JsonResult(new { success = false, message = "User identification failed." });
                }

                // Set creation metadata
                formModel.NewsArticle.CreatedDate = DateTime.Now;
                formModel.NewsArticle.CreatedById = accountId;
                formModel.NewsArticle.NewsStatus = formModel.IsPublished;

                // Process tags
                formModel.NewsArticle.Tags = await ProcessTagsAsync(formModel.TagsInput);

                // Add the article
                await _newsArticleService.AddNewsArticleAsync(formModel.NewsArticle);

                // Notify clients via SignalR
                //await NotifyClientsOfNewArticle(formModel.NewsArticle);

                return new JsonResult(new { success = true, message = "Article created successfully." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error creating article: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostUpdateNewsArticleAsync([FromForm] NewsArticleFormModel formModel)
        {
            try
            {
                // Get existing article
                var existingArticle = await _newsArticleService.GetNewsArticleByIdAsync(formModel.NewsArticle.NewsArticleId);
                if (existingArticle == null)
                {
                    return new JsonResult(new { success = false, message = "Article not found" });
                }

                // Preserve creation info
                formModel.NewsArticle.CreatedDate = existingArticle.CreatedDate;
                formModel.NewsArticle.CreatedById = existingArticle.CreatedById;

                // Update metadata
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out short accountId))
                {
                    formModel.NewsArticle.UpdatedById = accountId;
                }
                formModel.NewsArticle.ModifiedDate = DateTime.Now;
                formModel.NewsArticle.NewsStatus = formModel.IsPublished;

                // Process tags
                formModel.NewsArticle.Tags = await ProcessTagsAsync(formModel.TagsInput);

                // Update the article
                await _newsArticleService.UpdateNewsArticleAsync(formModel.NewsArticle);

                // Notify clients via SignalR
                //await NotifyClientsOfUpdatedArticle(formModel.NewsArticle);

                return new JsonResult(new { success = true, message = "Article updated successfully." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error updating article: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string newsArticleId)
        {
            try
            {
                if (string.IsNullOrEmpty(newsArticleId))
                {
                    TempData["ErrorMessage"] = "No article ID provided.";
                    return RedirectToPage();
                }

                // First check if the article exists
                var article = await _newsArticleService.GetNewsArticleByIdAsync(newsArticleId);
                if (article == null)
                {
                    TempData["ErrorMessage"] = "Article not found.";
                    return RedirectToPage();
                }

                // Delete the article
                await _newsArticleService.DeleteNewsArticleByIdAsync(newsArticleId);

                TempData["SuccessMessage"] = "Article deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting article: {ex.Message}";
                return RedirectToPage();
            }
        }


        private async Task<List<Tag>> ProcessTagsAsync(string tagsInput)
        {
            var tags = new List<Tag>();
            if (string.IsNullOrEmpty(tagsInput))
            {
                return tags;
            }

            var tagNames = tagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Distinct()
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            foreach (var tagName in tagNames)
            {
                var existingTag = await _tagService.GetTagByNameAsync(tagName);
                if (existingTag == null)
                {
                    existingTag = new Tag
                    {
                        TagName = tagName
                    };
                    await _tagService.AddTagAsync(existingTag);
                }
                tags.Add(existingTag);
            }

            return tags;
        }

    }
}
