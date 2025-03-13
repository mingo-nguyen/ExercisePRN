using FUBusiness.Entities;
using FURepositories.Interfaces;

namespace FUDataAccess.Services
{
    public class EnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;


        public EnrollmentService(IEnrollmentRepository enrollmentRepository, ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
        }


        public async Task<IEnumerable<EnrollmentRecords>> GetUserEnrollmentAsync(int userId)
        {
            return await _enrollmentRepository.GetByUserIdAsync(userId);
        }


        public async Task<IEnumerable<EnrollmentRecords>> GetCourseEnrollmentAsync(int courseId)
        {
            return await _enrollmentRepository.GetByCourseIdAsync(courseId);
        }

        public  async Task<EnrollmentRecords> EnrollInCourseAnsync(int userId, int courseId)
        {
            if(await _enrollmentRepository.isEnrolledAsync(userId, courseId))
            {
                throw new ArgumentException("User is already enrolled in this course");
            }
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course.Capacity <= 0)
            {
                throw new ArgumentException("Course is full");
            }
            var enrollment = new EnrollmentRecords
            {
                UserId = userId,
                CourseId = courseId,
                EntrollDate = DateTime.Now,
                Dropped = false
            };
            course.Capacity--;
            await _courseRepository.UpdateAsync(course);
            return await _enrollmentRepository.AddAsync(enrollment);
        }

        public async Task DropEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if(enrollment != null)
            {
                enrollment.Dropped = true;
                await _enrollmentRepository.UpdateAsync(enrollment);
                var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
                course.Capacity++;
                await _courseRepository.UpdateAsync(course);
            }
        }

    }
}
