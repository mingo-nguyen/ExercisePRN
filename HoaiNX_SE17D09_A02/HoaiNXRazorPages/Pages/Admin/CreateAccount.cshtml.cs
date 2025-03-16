using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HoaiNXRazorPages.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class CreateAccountModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public CreateAccountModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [BindProperty]
        public SystemAccount Account { get; set; }



        public async Task<IActionResult> OnPostAsync(SystemAccount Account)
        {
            this.Account = Account;
            if(!ModelState.IsValid)
            {
                this.Account = Account;
                return Page();
            }
            try
            {
                await _systemAccountService.AddAccountAsync(Account);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Accounts");
        }


        public void OnGet()
        {
        }
    }
}
