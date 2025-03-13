using FUBusiness.Entities;
using FUDataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using SP25_PRN222.WebApp.Filter;
using SP25_PRN222.WebApp.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP25_PRN222.WebApp.Controllers
{
    [AuthorizeRole("Admin")]
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;

        public CourseController(CourseService courseService, EnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        // GET: Course
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var enrollments = await _enrollmentService.GetCourseEnrollmentAsync(id);

            var model = new CourseDetailsAdminViewModel
            {
                Course = course,
                Enrollments = enrollments,
                ActiveEnrollments = enrollments.Count(e => !e.Dropped),
                DroppedEnrollments = enrollments.Count(e => e.Dropped)
            };

            return View(model);
        }

        // GET: Course/Create
        public IActionResult Create()
        {
            return View(new CourseViewModel());
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        Title = model.Title,
                        CourseCode = model.CourseCode,
                        Category = model.Category,
                        Capacity = model.Capacity,
                        CreatedAt = DateTime.Now
                    };

                    await _courseService.AddCourseAsync(course);
                    TempData["SuccessMessage"] = "Course created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the course");
                }
            }

            return View(model);
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseViewModel
            {
                ID = course.ID,
                Title = course.Title,
                CourseCode = course.CourseCode,
                Category = course.Category,
                Capacity = course.Capacity
            };

            return View(model);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        ID = model.ID,
                        Title = model.Title,
                        CourseCode = model.CourseCode,
                        Category = model.Category,
                        Capacity = model.Capacity
                    };

                    await _courseService.UpdateCourseAsync(course);
                    TempData["SuccessMessage"] = "Course updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the course");
                }
            }

            return View(model);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                TempData["SuccessMessage"] = "Course deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the course";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Course/Enrollments/5
        public async Task<IActionResult> Enrollments(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var enrollments = await _enrollmentService.GetCourseEnrollmentAsync(id);

            ViewData["CourseTitle"] = course.Title;
            ViewData["CourseId"] = course.ID;

            return View(enrollments);
        }
    }
}
