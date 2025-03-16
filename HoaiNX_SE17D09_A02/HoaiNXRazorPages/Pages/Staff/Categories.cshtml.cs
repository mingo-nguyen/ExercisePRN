using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace HoaiNXRazorPages.Pages.Staff
{
    [Authorize(Policy = "RequireStaffRole")]
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly INewsArticleService _articleService;
        private readonly ILogger<CategoriesModel> _logger;

        public CategoriesModel(
            ICategoryService categoryService,
            INewsArticleService articleService,
            ILogger<CategoriesModel> logger)
        {
            _categoryService = categoryService;
            _articleService = articleService;
            _logger = logger;
        }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        [BindProperty]
        public Category Category { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Categories = await _categoryService.GetCategoriesAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["ErrorMessage"] = "An error occurred while loading categories.";
                Categories = new List<Category>();
                return Page();
            }
        }

        // This handler returns a specific category by ID for the edit modal
        public async Task<IActionResult> OnGetCategoryByIdAsync(short categoryId)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Category not found." });
                }

                return new JsonResult(new { success = true, category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", categoryId);
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // This handler creates a new category
        public async Task<IActionResult> OnPostCreateCategoryAsync([FromForm] Category category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category.CategoryName))
                {
                    return new JsonResult(new { success = false, message = "Category name is required." });
                }

                // Check if category with same name already exists
                var existingCategories = await _categoryService.GetCategoriesAsync();
                if (existingCategories.Any(c => c.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    return new JsonResult(new { success = false, message = "A category with this name already exists." });
                }

                await _categoryService.AddCategoryAsync(category);
                return new JsonResult(new { success = true, message = "Category created successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {CategoryName}", category.CategoryName);
                return new JsonResult(new { success = false, message = $"Error creating category: {ex.Message}" });
            }
        }

        // This handler edits an existing category
        public async Task<IActionResult> OnPostEditCategoryAsync([FromForm] Category category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category.CategoryName))
                {
                    return new JsonResult(new { success = false, message = "Category name is required." });
                }

                // Check if category exists
                var existingCategory = await _categoryService.GetCategoryByIdAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    return new JsonResult(new { success = false, message = "Category not found." });
                }

                // Check if name is changed and if new name already exists
                if (!existingCategory.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase))
                {
                    var categories = await _categoryService.GetCategoriesAsync();
                    if (categories.Any(c => c.CategoryId != category.CategoryId &&
                                       c.CategoryName.Equals(category.CategoryName, StringComparison.OrdinalIgnoreCase)))
                    {
                        return new JsonResult(new { success = false, message = "A category with this name already exists." });
                    }
                }

                await _categoryService.UpdateCategoryAsync(category);
                return new JsonResult(new { success = true, message = "Category updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {CategoryId}", category.CategoryId);
                return new JsonResult(new { success = false, message = $"Error updating category: {ex.Message}" });
            }
        }

        // This handler deletes a category
        public async Task<IActionResult> OnPostDeleteAsync(short categoryId)
        {
            try
            {
                // Check if category is used in any articles
                var articles = await _articleService.GetNewsArticlesByCategoryAsync(categoryId);
                if (articles.Any())
                {
                    TempData["ErrorDeleteMessage"] = "Cannot delete category. It is used in one or more news articles.";
                    return RedirectToPage();
                }

                // Delete the category if not used
                await _categoryService.DeleteCategoryByIdAsync(categoryId);
                TempData["SuccessDeleteMessage"] = "Category deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {CategoryId}", categoryId);
                TempData["ErrorDeleteMessage"] = $"Error deleting category: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}
