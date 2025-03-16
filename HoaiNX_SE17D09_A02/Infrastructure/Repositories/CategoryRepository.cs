

using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HoaiNXRazorPages.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyDbContext _context;
        public CategoryRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryByIdAsync(short id)
        {
           _context.Categories.Remove(_context.Categories.Find(id));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(short categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(category.CategoryId);
            _context.Entry(categoryToUpdate).CurrentValues.SetValues(category);
            await _context.SaveChangesAsync();
        }
    }
}
