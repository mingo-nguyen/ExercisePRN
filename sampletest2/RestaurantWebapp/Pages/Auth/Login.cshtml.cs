using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RestaurantWebapp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly AuthService _authService;
        public LoginModel(ILogger<LoginModel> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [BindProperty]
        public LoginInput LoginData { get; set; } = new LoginInput();

        public string? ErrorMessage { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            // Store the return URL in ViewData to maintain it during postback
            ViewData["ReturnUrl"] = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _authService.AuthenticateUserAsync(LoginData.Username, LoginData.Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = LoginData.RememberMe,
                RedirectUri = returnUrl
            };
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);
            _logger.LogInformation("User {Username} logged in at {Time}", user.Username, DateTime.Now);
            return LocalRedirect(returnUrl);

        }


        public class LoginInput
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }
    }
}
