using D09AppRazor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace D09AppRazor.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get; set; } = new List<Student>();

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AgeFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AgeTo { get; set; }

        public async Task OnGetAsync()
        {
            await LoadStudentsAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            await LoadStudentsAsync();
            return Page();
        }

        private async Task LoadStudentsAsync()
        {
            var students = from s in _context.Students
                           select s;

            if (!string.IsNullOrEmpty(SearchName))
            {
                students = students.Where(s => s.Name.Contains(SearchName));
            }

            if (AgeFrom.HasValue)
            {
                students = students.Where(s => s.Age >= AgeFrom.Value);
            }

            if (AgeTo.HasValue)
            {
                students = students.Where(s => s.Age <= AgeTo.Value);
            }

            Student = await students.ToListAsync();
        }
    }
}
