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
    public class EditTableModel : PageModel
    {
        private readonly ITableService _tableService;

        public EditTableModel(ITableService tableService)
        {
            _tableService = tableService;
        }

        [BindProperty]
        public Table Table { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Table = await _tableService.GetAsync(id);

            if (Table == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Custom validation
            if (Table.Seats < 1)
            {
                ModelState.AddModelError("Table.Seats", "The number of seats must be at least 1.");
            }

            // Check for unique table number (excluding the current table)
            var allTables = await _tableService.GetAllAsync();
            if (allTables.Any(t => t.TableNumber == Table.TableNumber && t.Id != Table.Id))
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
                // Get current entity from database to avoid tracking conflicts
                var existingTable = await _tableService.GetAsync(Table.Id);
                if (existingTable == null)
                {
                    return NotFound();
                }

                // Update only the properties that need to be changed
                existingTable.TableNumber = Table.TableNumber;
                existingTable.Seats = Table.Seats;
                existingTable.Status = Table.Status;

                await _tableService.UpdateAsync(existingTable);
                TempData["SuccessMessage"] = $"Table #{Table.TableNumber} was updated successfully.";
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
