using Microsoft.AspNetCore.Mvc;
using FUDataAccess.Services;
using SP25_PRN222.WebApp.Models.CourseModels;
using FUBusiness.Entities;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using SP25_PRN222.WebApp.Filter;


namespace SP25_PRN222.WebApp.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;

        public AdminController(CourseService courseService, EnrollmentService enrollmentService)
        {
            this._courseService = courseService;
            this._enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Course course = toCourse(model);
                await _courseService.AddCourseAsync(course);
                return RedirectToAction(nameof(Index));
            }
            catch (System.ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var course = await _courseService.GetCourseByIdAsync(model.ID);
                if (course == null)
                {
                    return NotFound();
                }
                course = toCourse(model);
                await _courseService.UpdateCourseAsync(course);
                return RedirectToAction(nameof(Index));
            }
            catch (System.ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           await _courseService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Enrollments(int id)
        {
            var enrollments = await _enrollmentService.GetCourseEnrollmentAsync(id);
            return View(enrollments);
        }





        private Course toCourse(CourseViewModel model)
        {
            return new Course
            {
                Title = model.Title,
                CourseCode = model.CourseCode,
                Category = model.Category,
                Capacity = model.Capacity
            };
        }

    }
}
