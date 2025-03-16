
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace HoaiNXRazorPages.Infrastructure.Repositories
{
    
    public class NewsArticleRepository : INewsArticleRepository
    {

        private readonly MyDbContext _context;

        public NewsArticleRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task AddNewsArticleAsync(NewsArticle newsArticle)
        {
            newsArticle.CreatedDate = DateTime.Now;
            await _context.NewsArticles.AddAsync(newsArticle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNewsArticleByIdAsync(string id)
        {
            var newsArticle = await _context.NewsArticles
        .Include(a => a.Tags)
        .FirstOrDefaultAsync(a => a.NewsArticleId == id);
           
            if(newsArticle.Tags != null && newsArticle.Tags.Any())
            {
                newsArticle.Tags.Clear();
            }

            _context.NewsArticles.Remove(newsArticle);
            await _context.SaveChangesAsync();
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId)
        {
            var article = await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId);

            if(article != null)
            {
                await _context.Entry(article)
                    .Collection(n => n.Tags)
                    .LoadAsync();
            }
            return article;

        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesByCategoryAsync(int categoryId)
        {
            return await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .Include(n => n.Category)
                .Where(n => n.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var endDateTime = endDate.Date.AddDays(1).AddTicks(-1);
            var listNews = await _context.NewsArticles
                .Include(n => n.CreatedBy)
                .Include(n => n.Category)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
            return listNews ?? new List<NewsArticle>();
        }

        public async Task<IEnumerable<Tag>> GetTagsForArticleAsync(string newsArticleId)
        {
            var tags = await _context.Tags
                       .Where(t => t.NewsArticles.Any(na => na.NewsArticleId == newsArticleId))
                       .ToListAsync();

            return tags;
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            var existingArticle = await _context.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.NewsArticleId == newsArticle.NewsArticleId);

            if (existingArticle == null)
                throw new KeyNotFoundException($"Article with ID {newsArticle.NewsArticleId} not found");

            // Update basic properties
            _context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);
            existingArticle.ModifiedDate = DateTime.Now;

            // Handle tags - clear and re-add
            var existingTags = await _context.Entry(existingArticle)
                .Collection(a => a.Tags)
                .Query()
                .ToListAsync();

            foreach (var tag in existingTags)
            {
                existingArticle.Tags.Remove(tag);
            }

            // Add new tags
            if (newsArticle.Tags != null)
            {
                foreach (var tag in newsArticle.Tags)
                {
                    // Find existing tag by name or create new one
                    var existingTag = await _context.Tags
                        .FirstOrDefaultAsync(t => t.TagName == tag.TagName);

                    if (existingTag == null)
                    {
                        // Create a new tag if it doesn't exist
                        existingTag = new Tag
                        {
                            TagId = tag.TagId == 0 ? await GetNextTagIdAsync() : tag.TagId,
                            TagName = tag.TagName,
                            Note = tag.Note
                        };

                        await _context.Tags.AddAsync(existingTag);
                        await _context.SaveChangesAsync(); // Save to get the new tag ID
                    }

                    existingArticle.Tags.Add(existingTag);
                }
            }

            await _context.SaveChangesAsync();
        }

        // Helper method to get the next TagId
        private async Task<int> GetNextTagIdAsync()
        {
            var maxTagId = await _context.Tags
                .MaxAsync(t => (int?)t.TagId) ?? 0;
            return maxTagId + 1;
        }

    }
}
