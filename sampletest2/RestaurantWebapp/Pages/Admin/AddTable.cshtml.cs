using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantWebapp.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AddTableModel : PageModel
    {
        private readonly ITableService _tableService;

        public AddTableModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty]
        public Table Table { get; set; } = new Table
        {
            Status = "Available",
            Seats = 2
        };

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Just return the page with default values
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Custom validation
            if (Table.Seats < 1)
            {
                ModelState.AddModelError("Table.Seats", "The number of seats must be at least 1.");
            }

            // Check for unique table number
            var allTables = await _tableService.GetAllAsync();
            if (allTables.Any(t => t.TableNumber == Table.TableNumber))
            {
                ModelState.AddModelError("Table.TableNumber", "This table number is already in use.");
                ErrorMessage = "A table with this number already exists.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _tableService.AddAsync(Table);
                TempData["SuccessMessage"] = $"Table #{Table.TableNumber} was added successfully.";
                return RedirectToPage("/Admin/ManageTables");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
