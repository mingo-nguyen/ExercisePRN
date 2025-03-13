using FUBusiness.Entities;

namespace FURepositories.Interfaces
{
    public interface ISessionRepository
    {
        Task<Sessions> GetByIdAsync(string sessionId);
        Task<IEnumerable<Sessions>> GetAllAsync();
        Task<Sessions> GetByUserIdAsync(int userId);
        Task<Sessions> AddAsync(Sessions session);
        Task UpdateAsync(Sessions session);
        Task DeleteAsync(string id);
        Task DeleteByUserIdAsync(int userId);
        Task<bool> ExistsAsync(string sessionId);
    }
}
