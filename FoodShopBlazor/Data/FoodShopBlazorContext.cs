using Microsoft.EntityFrameworkCore;
using FoodShopBlazor.Models;

namespace FoodShopBlazor.Data
{
    public class FoodShopBlazorContext : DbContext
    {
        public FoodShopBlazorContext(DbContextOptions<FoodShopBlazorContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<Restaurant> Restaurant { get; set; } = default!;
        public DbSet<OrderDetail> OrderDetail { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Food>()
                .HasOne(f => f.Restaurant)
                .WithMany(r => r.Menu)
                .HasForeignKey(f => f.RestaurantId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Food)
                .WithMany()
                .HasForeignKey(od => od.FoodId);
        }
    }
}
