using HoaiNXRazorPages.Domain.Entities;
using System;

namespace HoaiNXRazorPages.Domain.RepositoryContracts
{
    public interface INewsArticleRepository
    {
        Task<IEnumerable<NewsArticle>> GetNewsArticlesAsync();
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId);
        Task AddNewsArticleAsync(NewsArticle newsArticle);
        Task UpdateNewsArticleAsync(NewsArticle newsArticle);
        Task DeleteNewsArticleByIdAsync(string id);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByCategoryAsync(int categoryId);

        Task<IEnumerable<Tag>> GetTagsForArticleAsync(string newsArticleId);

    }
}
