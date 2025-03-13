using FUBusiness.Entities;
using FUDataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using SP25_PRN222.WebApp.Filter;
using SP25_PRN222.WebApp.Models.StudentModels;
using System;
using System.Threading.Tasks;

namespace SP25_PRN222.WebApp.Controllers
{
    [AuthorizeRole("Admin")]
    public class StudentManagementController : Controller
    {
        private readonly UserService _userService;
        private readonly EnrollmentService _enrollmentService;

        public StudentManagementController(UserService userService, EnrollmentService enrollmentService)
        {
            _userService = userService;
            _enrollmentService = enrollmentService;
        }

        // GET: /StudentManagement
        public async Task<IActionResult> Index()
        {
            var students = await _userService.GetAllStudentsAsync();
            return View(students);
        }

        // GET: /StudentManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _userService.GetUserByIdAsync(id);
            if (student == null || student.Role != "Student")
            {
                return NotFound();
            }

            var enrollments = await _enrollmentService.GetUserEnrollmentAsync(id);

            var viewModel = new StudentDetailsViewModel
            {
                Student = student,
                Enrollments = enrollments
            };

            return View(viewModel);
        }

        // GET: /StudentManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /StudentManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Password = _userService.HashPassword(model.Password),
                        Role = "Student",
                        CreatedAt = DateTime.Now
                    };

                    await _userService.AddUserAsync(student);
                    TempData["SuccessMessage"] = "Student created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        // GET: /StudentManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _userService.GetUserByIdAsync(id);
            if (student == null || student.Role != "Student")
            {
                return NotFound();
            }

            var viewModel = new StudentEditViewModel
            {
                ID = student.ID,
                FullName = student.FullName,
                Email = student.Email
            };

            return View(viewModel);
        }

        // POST: /StudentManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _userService.GetUserByIdAsync(model.ID);
                    if (student == null || student.Role != "Student")
                    {
                        return NotFound();
                    }

                    student.FullName = model.FullName;
                    student.Email = model.Email;

                    // Only update password if a new one is provided
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        student.Password = _userService.HashPassword(model.NewPassword);
                    }

                    await _userService.UpdateUserAsync(student);
                    TempData["SuccessMessage"] = "Student updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        // POST: /StudentManagement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _userService.GetUserByIdAsync(id);
            if (student == null || student.Role != "Student")
            {
                return NotFound();
            }

            try
            {
                // First check if student has enrollments
                var enrollments = await _enrollmentService.GetUserEnrollmentAsync(id);
                if (enrollments.Any(e => !e.Dropped))
                {
                    TempData["ErrorMessage"] = "Cannot delete student with active enrollments. Please drop all enrollments first.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await _userService.DeleteUserAsync(id);
                TempData["SuccessMessage"] = "Student deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
