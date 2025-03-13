using FUBusiness.Data;
using FUBusiness.Entities;
using FURepositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FURepositories.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly FUDbContext _context;

        public CourseRepository(FUDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchAsync(string title, string category)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(c => c.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(c => c.Category == category);
            }

            return await query
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        public async Task<Course> AddAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            // Check if course code already exists
            if (await CourseCodeExistsAsync(course.CourseCode))
                throw new InvalidOperationException($"A course with code {course.CourseCode} already exists.");

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return course;
        }

        public async Task UpdateAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            // Check if the course exists
            var existingCourse = await _context.Courses.FindAsync(course.ID);
            if (existingCourse == null)
                throw new InvalidOperationException($"Course with ID {course.ID} not found.");

            // Check if course code already exists (but only if it's different from the current one)
            if (existingCourse.CourseCode != course.CourseCode && await CourseCodeExistsAsync(course.CourseCode))
                throw new InvalidOperationException($"A course with code {course.CourseCode} already exists.");

            // Update only certain properties to avoid overwriting navigation properties
            existingCourse.Title = course.Title;
            existingCourse.Category = course.Category;
            existingCourse.CourseCode = course.CourseCode;
            existingCourse.Capacity = course.Capacity;

            // Specify entity state as modified
            _context.Entry(existingCourse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(course.ID))
                    throw new InvalidOperationException($"Course with ID {course.ID} not found.");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (course == null)
                return;

            // Check if course has enrollments
            if (course.Enrollments.Any(e => !e.Dropped))
                throw new InvalidOperationException("Cannot delete course with active enrollments.");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(c => c.ID == id);
        }

        public async Task<bool> CourseCodeExistsAsync(string courseCode)
        {
            if (string.IsNullOrEmpty(courseCode))
                throw new ArgumentNullException(nameof(courseCode));

            return await _context.Courses.AnyAsync(c => c.CourseCode == courseCode);
        }

        // Additional useful methods

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            return await _context.Courses
                .Select(c => c.Category)
                .Distinct()
                .Where(c => !string.IsNullOrEmpty(c))
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<int> GetEnrollmentCountAsync(int courseId)
        {
            return await _context.EnrollmentRecords
                .Where(e => e.CourseId == courseId && !e.Dropped)
                .CountAsync();
        }

        public async Task<bool> DecrementCapacityAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null || course.Capacity <= 0)
                return false;

            course.Capacity--;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementCapacityAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            course.Capacity++;
            await _context.SaveChangesAsync();
            return true;
        }


        public Task<Course> GetByCodeAsync(string courseCode)
        {
            throw new NotImplementedException();
        }
    }
}
