using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class ManageTablesModel : PageModel
    {
        private readonly ITableService _tableService;

        public ManageTablesModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty(SupportsGet = true)]
        public int SearchSeats { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchStatus { get; set; }

        public IEnumerable<Table> Tables { get; set; } = Enumerable.Empty<Table>();

        public async Task OnGetAsync()
        {
            var allTables = await _tableService.GetAllAsync();

            // Apply filters if provided
            var filteredTables = allTables;

            if (SearchSeats > 0)
            {
                filteredTables = filteredTables.Where(t => t.Seats >= SearchSeats);
            }

            if (!string.IsNullOrEmpty(SearchStatus))
            {
                filteredTables = filteredTables.Where(t => t.Status == SearchStatus);
            }

            Tables = filteredTables;
        }
    }
}
