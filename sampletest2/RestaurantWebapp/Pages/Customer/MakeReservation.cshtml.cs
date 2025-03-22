using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages.Customer
{
    [Authorize(Policy = "CustomerOnly")]
    public class MakeReservationModel : PageModel
    {
        private readonly ITableService _tableService;
        private readonly IReservationService _reservationService;

        public MakeReservationModel(
            ITableService tableService,
            IReservationService reservationService)
        {
            _tableService = tableService;
            _reservationService = reservationService;
        }

        [BindProperty(SupportsGet = true)]
        public int TableId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime ReservationDate { get; set; }

        [BindProperty]
        public string ReservationTime { get; set; } = "18:00";

        [BindProperty]
        public string SpecialRequests { get; set; }

        public Table Table { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Table = await _tableService.GetAsync(TableId);

            if (Table == null)
            {
                return NotFound("Table not found.");
            }

            if (Table.Status != "Available")
            {
                ErrorMessage = "This table is no longer available for reservation.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Table = await _tableService.GetAsync(TableId);
                return Page();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Parse time
            var timeComponents = ReservationTime.Split(':');
            var hours = int.Parse(timeComponents[0]);
            var minutes = int.Parse(timeComponents[1]);

            var reservationDateTime = ReservationDate.AddHours(hours).AddMinutes(minutes);

            var reservation = new Reservation
            {
                UserId = userId,
                TableId = TableId,
                ReservationTime = reservationDateTime
            };

            try
            {
                await _reservationService.AddAsync(reservation);
                TempData["SuccessMessage"] = "Your reservation has been confirmed!";
                return RedirectToPage("/Customer/MyReservations");
            }
            catch (InvalidOperationException ex)
            {
                Table = await _tableService.GetAsync(TableId);
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
