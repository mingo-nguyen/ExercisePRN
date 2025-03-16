using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaiNXRazorPages.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AccountsModel : PageModel
    {
       private readonly ISystemAccountService systemAccountService;
        public AccountsModel(ISystemAccountService systemAccountService)
        {
            this.systemAccountService = systemAccountService;
        }



        public IEnumerable<SystemAccount> SystemAccounts
        {
            get; set;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            SystemAccounts = await systemAccountService.GetAccountsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(short accountId)
        {
            var account = await systemAccountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }
            await systemAccountService.DeleteAccountAsync(account);
            return RedirectToPage();
        }


    }
}
