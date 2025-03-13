using FUBusiness.Entities;
using FURepositories.Interfaces;

namespace FUDataAccess.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task<Course> GetCourseByCode(string code)
        {
            return await _courseRepository.GetByCodeAsync(code);
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            if(string.IsNullOrEmpty(course.Title))
            {
                throw new ArgumentException("Title is required");
            }
            if(course.Capacity < 0)
            {
                throw new ArgumentException("Capacity must be greater than or equal to 0");
            }
            if(await _courseRepository.CourseCodeExistsAsync(course.CourseCode))
            {
                throw new ArgumentException("Course code already exists");
            }
            return await _courseRepository.AddAsync(course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            if (string.IsNullOrEmpty(course.Title))
            {
                throw new ArgumentException("Title is required");
            }
            if (course.Capacity < 0)
            {
                throw new ArgumentException("Capacity must be greater than or equal to 0");
            }
            var existingCourse = await _courseRepository.GetByIdAsync(course.ID);
            if (existingCourse.CourseCode != course.CourseCode && 
                await _courseRepository.CourseCodeExistsAsync(course.CourseCode))
            {
                throw new ArgumentException("Course code already exists");
            }

            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _courseRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Course>> SearchCourseAsync(string title, string category)
        {
            return await _courseRepository.SearchAsync(title, category);
        }


    }
}
