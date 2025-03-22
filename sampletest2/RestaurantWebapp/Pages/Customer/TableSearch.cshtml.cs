using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages.Customer
{
    [Authorize(Policy = "CustomerOnly")]
    public class TableSearchModel : PageModel
    {
        private readonly ITableService _tableService;

        public TableSearchModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty(SupportsGet = true)]
        [Range(1, 20, ErrorMessage = "Please enter a valid number of seats (1-20)")]
        public int SeatsRequired { get; set; } = 2;

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime ReservationDate { get; set; } = DateTime.Today;

        public IEnumerable<Table> AvailableTables { get; set; } = Enumerable.Empty<Table>();

        public bool HasSearched { get; set; } = false;

        public async Task OnGetAsync()
        {
            // Only perform search if seats have been specified
            if (SeatsRequired > 0)
            {
                HasSearched = true;
                var allTables = await _tableService.GetAllAsync();

                // Filter tables based on seat count and availability
                AvailableTables = allTables.Where(t =>
                    t.Seats >= SeatsRequired &&
                    t.Status == "Available" &&
                    !t.Reservations.Any(r => r.ReservationTime.Date == ReservationDate.Date));
            }
        }
    }
}
