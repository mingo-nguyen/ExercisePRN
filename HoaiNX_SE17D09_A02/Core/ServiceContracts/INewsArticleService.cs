

using HoaiNXRazorPages.Domain.Entities;

namespace HoaiNXRazorPages.Application.ServiceContracts
{
    public  interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetNewsArticlesAsync();
        Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId);
        Task AddNewsArticleAsync(NewsArticle newsArticle);
        Task UpdateNewsArticleAsync(NewsArticle newsArticle);
        Task DeleteNewsArticleByIdAsync(string id);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByCategoryAsync(int categoryId);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByTagAsync(string tag);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByAuthorAsync(string author);
        Task<IEnumerable<NewsArticle>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Tag>> GetTagsForArticleAsync(string newsArticleId);

    }
}
