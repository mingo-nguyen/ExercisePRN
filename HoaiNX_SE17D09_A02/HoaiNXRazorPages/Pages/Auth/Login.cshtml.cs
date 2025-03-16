
using HoaiNXRazorPages.Application.ServiceContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HoaiNXRazorPages.Domain.RepositoryContracts;

namespace HoaiNXRazorPages.Pages.Auth
{
    public class LoginModel : PageModel
    {
       
        private readonly  IAuthenticationUserService _authenticationUserService;


        public LoginModel(ISystemAccountRepository systemAccountRepository, IAuthenticationUserService authenticationService)
        {
            _authenticationUserService = authenticationService;
        }

        [BindProperty]
        public LoginInputModel Input
        {
            get; set;
        }


        public class LoginInputModel
        {
            [Required]
            public string Username { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _authenticationUserService.AuthenticateUserAsync(Input.Username, Input.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();

            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, user.AccountRole.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString())
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            if(user.AccountRole == 0)
            {
                return RedirectToPage("/Admin/Index");
            }
            else if(user.AccountRole == 1)
            {
                return RedirectToPage("/Staff/Index");
            }
                return RedirectToPage("/News/Index");

        }


    }
}
