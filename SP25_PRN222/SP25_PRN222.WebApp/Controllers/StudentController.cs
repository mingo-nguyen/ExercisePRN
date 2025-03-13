using FUBusiness.Entities;
using FUDataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using SP25_PRN222.WebApp.Filter;
using SP25_PRN222.WebApp.Models.CourseModels;
using SP25_PRN222.WebApp.Models.EnrollmentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP25_PRN222.WebApp.Controllers
{
    [AuthorizeRole("Student")]
    public class StudentController : Controller
    {
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;
        private readonly UserService _userService;

        public StudentController(CourseService courseService, EnrollmentService enrollmentService, UserService userService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _userService = userService;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var enrollments = await _enrollmentService.GetUserEnrollmentAsync(userId);

            // Filter to active enrollments only
            var activeEnrollments = enrollments.Where(e => !e.Dropped).ToList();

            return View(activeEnrollments);
        }

        // GET: Student/Courses
        public async Task<IActionResult> Courses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var enrollments = await _enrollmentService.GetUserEnrollmentAsync(userId);

            // Create view models that include enrollment status
            var courseViewModels = courses.Select(c => new CourseEnrollmentViewModel
            {
                Course = c,
                IsEnrolled = enrollments.Any(e => e.CourseId == c.ID && !e.Dropped),
                HasAvailableCapacity = c.Capacity > 0
            }).ToList();

            return View(courseViewModels);
        }

        // GET: Student/CourseDetails/5
        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var enrollment = (await _enrollmentService.GetUserEnrollmentAsync(userId))
                .FirstOrDefault(e => e.CourseId == id && !e.Dropped);

            var viewModel = new CourseDetailsViewModel
            {
                Course = course,
                IsEnrolled = enrollment != null,
                EnrollmentId = enrollment?.Id ?? 0
            };

            return View(viewModel);
        }

        // POST: Student/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                await _enrollmentService.EnrollInCourseAnsync(userId, courseId);
                TempData["SuccessMessage"] = "Successfully enrolled in course";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Courses));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while enrolling in the course";
                return RedirectToAction(nameof(Courses));
            }
        }

        // POST: Student/DropCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DropCourse(int id)
        {
            try
            {
                await _enrollmentService.DropEnrollmentAsync(id);
                TempData["SuccessMessage"] = "Successfully dropped the course";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while dropping the course";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Student/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = await _userService.GetUserByIdAsync(userId);
            return View(user);
        }

        // GET: Student/EnrollmentHistory
        public async Task<IActionResult> EnrollmentHistory()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var enrollments = await _enrollmentService.GetUserEnrollmentAsync(userId);
            return View(enrollments);
        }
    }
}
