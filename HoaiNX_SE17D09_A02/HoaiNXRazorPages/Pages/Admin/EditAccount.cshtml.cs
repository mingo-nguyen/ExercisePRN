using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaiNXRazorPages.Pages.Admin
{
    [Authorize(policy:"RequireAdminRole")]
    public class EditAccountModel : PageModel
    {
        private readonly ISystemAccountService systemAccountService;
        public EditAccountModel(ISystemAccountService systemAccountService)
        {
            this.systemAccountService = systemAccountService;
        }

        [BindProperty]        
        public SystemAccount Account { get; set; }


        public async Task<IActionResult> OnGetAsync(short id)
        {
            Account = await systemAccountService.GetAccountByIdAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(SystemAccount account)
        {
            if (!ModelState.IsValid)
            {
                Account = account;
                return Page();

            }

            var existingUser = await systemAccountService.GetAccountByEmailAsync(account.AccountEmail);
            if(existingUser == null || existingUser.AccountId == account.AccountId)
            {
                await systemAccountService.UpdateAccountAsync(account);
                return Page();
            }
            else
            {
                ModelState.AddModelError("", "New Email already exists");
                return Page();
            }


        }

    }
}
