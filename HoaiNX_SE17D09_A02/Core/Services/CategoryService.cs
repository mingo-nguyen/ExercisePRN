using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await categoryRepository.AddCategoryAsync(category);

        }

        public async Task DeleteCategoryByIdAsync(short id)
        {
            await categoryRepository.DeleteCategoryByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await categoryRepository.GetCategoriesAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(short categoryId)
        {
            return await categoryRepository.GetCategoryByIdAsync(categoryId);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await categoryRepository.UpdateCategoryAsync(category);
        }
    }
}
