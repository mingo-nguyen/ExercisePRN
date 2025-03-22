using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages.Customer
{
    [Authorize(Policy = "CustomerOnly")]
    public class MyReservationsModel : PageModel
    {
        private readonly IReservationService _reservationService;

        public MyReservationsModel(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public IEnumerable<Reservation> Reservations { get; set; } = Enumerable.Empty<Reservation>();

        [BindProperty(SupportsGet = true)]
        public bool ShowPastReservations { get; set; } = false;

        public async Task OnGetAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var allReservations = await _reservationService.GetUserReservationsAsync(userId);

            if (ShowPastReservations)
            {
                // Past reservations
                Reservations = allReservations.Where(r => r.ReservationTime < DateTime.Now)
                                             .OrderByDescending(r => r.ReservationTime);
            }
            else
            {
                // Upcoming reservations
                Reservations = allReservations.Where(r => r.ReservationTime >= DateTime.Now)
                                             .OrderBy(r => r.ReservationTime);
            }
        }

        public async Task<IActionResult> OnPostCancelReservationAsync(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reservation = await _reservationService.GetByIdAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Ensure the reservation belongs to the current user
            if (reservation.UserId != userId)
            {
                return Forbid();
            }

            // Check if reservation is in the future and can be canceled
            if (reservation.ReservationTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Cannot cancel a past reservation.";
                return RedirectToPage();
            }

            // Check if the reservation is within a reasonable cancellation window (e.g., 2 hours)
            if ((reservation.ReservationTime - DateTime.Now).TotalHours < 2)
            {
                TempData["ErrorMessage"] = "Reservations must be canceled at least 2 hours in advance.";
                return RedirectToPage();
            }

            // Cancel the reservation
            await _reservationService.DeleteAsync(id);

            TempData["SuccessMessage"] = "Your reservation has been successfully canceled.";
            return RedirectToPage();
        }
    }
}
