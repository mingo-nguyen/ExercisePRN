using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages
{
    public class TableDetailsModel : PageModel
    {
        private readonly ITableService _tableService;
        private readonly IReservationService _reservationService;

        public TableDetailsModel(
            ITableService tableService,
            IReservationService reservationService)
        {
            _tableService = tableService;
            _reservationService = reservationService;
        }

        public Table Table { get; set; }

        public IEnumerable<Reservation> Reservations { get; set; } = new List<Reservation>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Table = await _tableService.GetAsync(id);

            if (Table == null)
            {
                return NotFound();
            }

            // If admin, also get the reservations for this table
            if (User.IsInRole("Admin"))
            {
                var allReservations = await _reservationService.GetAllAsync();
                Reservations = allReservations
                    .Where(r => r.TableId == id && r.ReservationTime >= DateTime.Now)
                    .OrderBy(r => r.ReservationTime)
                    .ToList();
            }

            return Page();
        }

        public string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Available" => "bg-success",
                "Reserved" => "bg-warning",
                "Occupied" => "bg-danger",
                "Maintenance" => "bg-secondary",
                _ => "bg-light"
            };
        }
    }
}
