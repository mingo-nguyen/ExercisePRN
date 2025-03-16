using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using Microsoft.AspNetCore.SignalR;
using HoaiNXRazorPages.Infrastructure.Hubs;

namespace HoaiNXRazorPages.Core.Services
{
    public class NewsArticleService : INewsArticleService
    {

        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IHubContext<NewsHub> _hubContext;

        public NewsArticleService(INewsArticleRepository newsArticleRepository,
            ICategoryRepository categoryRepository, ITagRepository tagRepository, IHubContext<NewsHub> hubContext)
        {
            _newsArticleRepository = newsArticleRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _hubContext = hubContext;
        }

        public async Task AddNewsArticleAsync(NewsArticle newsArticle)
        {
            await _newsArticleRepository.AddNewsArticleAsync(newsArticle);
            if(newsArticle.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(newsArticle.CategoryId.Value);
                newsArticle.Category = category;
            }

            await _hubContext.Clients.All.SendAsync("ReceiveNewNewsArticle", new
            {
                newsArticleId = newsArticle.NewsArticleId,
                newsTitle = newsArticle.NewsTitle,
                headline = newsArticle.Headline,
                createdDate = newsArticle.CreatedDate,
                categoryName = newsArticle.Category?.CategoryName,
                categoryId = newsArticle.CategoryId,
                status = newsArticle.NewsStatus
            });

        }

        public async Task DeleteNewsArticleByIdAsync(string id)
        {
            await _newsArticleRepository.DeleteNewsArticleByIdAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveDeletedNewsArticle", id);
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId)
        {
            return await _newsArticleRepository.GetNewsArticleByIdAsync(newsArticleId);
        }
        

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesAsync()
        {
            return await _newsArticleRepository.GetNewsArticlesAsync();
        }

        public Task<IEnumerable<NewsArticle>> GetNewsArticlesByAuthorAsync(string author)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesByCategoryAsync(int categoryId)
        {
            return await _newsArticleRepository.GetNewsArticlesByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _newsArticleRepository.GetNewsArticlesByDateRangeAsync(startDate, endDate);
        }

        public Task<IEnumerable<NewsArticle>> GetNewsArticlesByTagAsync(string tag)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tag>> GetTagsForArticleAsync(string newsArticleId)
        {
           return await _newsArticleRepository.GetTagsForArticleAsync(newsArticleId);
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            // Load the category for the news article if it has one
            if (newsArticle.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(newsArticle.CategoryId.Value);
                newsArticle.Category = category;
            }
            await _newsArticleRepository.UpdateNewsArticleAsync(newsArticle);
            // Notify clients about the updated news article
            await _hubContext.Clients.All.SendAsync("ReceiveUpdatedNewsArticle", new
            {
                newsArticleId = newsArticle.NewsArticleId,
                newsTitle = newsArticle.NewsTitle,
                headline = newsArticle.Headline,
                createdDate = newsArticle.CreatedDate,
                categoryName = newsArticle.Category?.CategoryName,
                categoryId = newsArticle.CategoryId,
                status = newsArticle.NewsStatus
            });
        }
    }
}
