using FUBusiness.Entities;

namespace FURepositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<EnrollmentRecords> GetByIdAsync(int id);
        Task<IEnumerable<EnrollmentRecords>> GetAllAsync();
        Task<IEnumerable<EnrollmentRecords>> GetByUserIdAsync(int userId);
        Task<IEnumerable<EnrollmentRecords>> GetByCourseIdAsync(int courseId);
        Task<EnrollmentRecords> AddAsync(EnrollmentRecords enrollment);
        Task UpdateAsync(EnrollmentRecords enrollment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> isEnrolledAsync(int userId, int courseId);
    }
}
