using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaiNXRazorPages.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnpostAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            {
                return RedirectToPage("/Auth/Login");

            }
        }
    }
}
