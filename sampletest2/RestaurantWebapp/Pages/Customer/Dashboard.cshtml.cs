using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantWebapp.Pages.Customer
{
    [Authorize(Policy = "CustomerOnly")]
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
