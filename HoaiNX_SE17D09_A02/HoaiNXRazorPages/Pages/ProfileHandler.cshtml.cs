using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HoaiNXRazorPages.Pages
{
    [Authorize]
    public class ProfileHandlerModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public ProfileHandlerModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public SystemAccount UserAccount { get; set; }

        // This handler returns the profile partial view
        public async Task<IActionResult> OnGetProfilePartialAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short accountId))
            {
                return new JsonResult(new { success = false, message = "User identification failed." });
            }

            UserAccount = await _accountService.GetAccountByIdAsync(accountId);

            if (UserAccount == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            // Clear password for security
            UserAccount.AccountPassword = string.Empty;

            return Partial("_ProfilePartial", UserAccount);
        }

        // This handler handles profile updates
        public async Task<IActionResult> OnPostUpdateProfileAsync(SystemAccount model)
        {
            if (string.IsNullOrEmpty(model?.AccountName))
            {
                return new JsonResult(new { success = false, message = "Name cannot be empty." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !short.TryParse(userId, out short accountId))
            {
                return new JsonResult(new { success = false, message = "User identification failed." });
            }

            var currentAccount = await _accountService.GetAccountByIdAsync(accountId);

            if (currentAccount == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            // Security check - ensure user is updating their own profile
            if (currentAccount.AccountId != model.AccountId)
            {
                return new JsonResult(new { success = false, message = "You can only update your own profile." });
            }

            // Update allowed fields only
            currentAccount.AccountName = model.AccountName;

            // Only update password if provided
            if (!string.IsNullOrEmpty(model.AccountPassword))
            {
                currentAccount.AccountPassword = model.AccountPassword;
            }

            try
            {
                await _accountService.UpdateAccountAsync(currentAccount);
                return new JsonResult(new
                {
                    success = true,
                    message = "Profile updated successfully.",
                    updatedName = currentAccount.AccountName
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
