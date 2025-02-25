using Microsoft.EntityFrameworkCore;

namespace D09AppRazor.Model
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Student> Students { get; set; }    
    }
}
