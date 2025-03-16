
using HoaiNXRazorPages.Domain.Entities;
using HoaiNXRazorPages.Domain.RepositoryContracts;


namespace HoaiNXRazorPages.Infrastructure.Repositories
{
    public class NewsTagRepository : INewsTagRepository
    {
        public Task AddNewsTagAsync(NewsTag newsTag)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNewsTagAsync(NewsTag newsTag)
        {
            throw new NotImplementedException();
        }

        public Task<NewsTag> GetNewsTagByIdAsync(int newsTagId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NewsTag>> GetNewsTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateNewsTagAsync(NewsTag newsTag)
        {
            throw new NotImplementedException();
        }
    }
}
