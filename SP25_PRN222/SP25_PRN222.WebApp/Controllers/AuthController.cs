using FUDataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SP25_PRN222.WebApp.Models.AuthModels;

namespace SP25_PRN222.WebApp.Controllers
{
    public class AuthController : Controller
    {

        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _authService.AuthenticateAsync(model.Email, model.Password);
            if(user == null)
            {
                ModelState.AddModelError("Email", "Invalid email or password");
                return View(model);
            }
            var session = await _authService.CreateSessionAsync(user.ID, user.Role);
            HttpContext.Session.SetString("sessionId", session.SessionId);
            HttpContext.Session.SetString("UserId", user.ID.ToString());
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            if(user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Student");
            }

        }

        public async Task<IActionResult> Logout()
        {
            var sessionId = HttpContext.Session.GetString("sessionId");
            if (sessionId != null)
            {
                await _authService.EndSessionAsync(sessionId);
                HttpContext.Session.Clear();
            }
            TempData["SuccessMessage"] = "You have been logged out successfully";
            return RedirectToAction("Login");
        }


    }
}
