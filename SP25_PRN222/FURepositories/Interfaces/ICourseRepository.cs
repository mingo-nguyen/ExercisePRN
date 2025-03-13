using FUBusiness.Entities;
namespace FURepositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> SearchAsync(string title, string category);
        Task<Course> AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CourseCodeExistsAsync(string courseCode);
        Task<Course> GetByCodeAsync(string courseCode);
    }
}
