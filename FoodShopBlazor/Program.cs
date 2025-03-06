using FoodShopBlazor.Components;
using FoodShopBlazor.Data;
using FoodShopBlazor.Hubs;
using Microsoft.EntityFrameworkCore;

namespace FoodShopBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Register DbContext
            builder.Services.AddDbContext<FoodShopBlazorContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FoodShopBlazor")));

            // Add QuickGrid support
            builder.Services.AddQuickGridEntityFrameworkAdapter();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register HttpClient with BaseAddress
            builder.Services.AddHttpClient("FoodShopBlazorAPI", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiBaseAddress"]);
            });
            builder.Services.AddScoped<FoodShopBlazor.Services.CartService>();
            builder.Services.AddSignalR();
            // Register API controllers
            builder.Services.AddControllers();
            builder.Services.AddDbContextFactory<FoodShopBlazorContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FoodShopBlazor")), ServiceLifetime.Transient);
            var app = builder.Build();
            app.MapHub<OrderHub>("/orderhub");
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
    app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Map API controllers
            app.MapControllers();

            app.Run();
        }
    }
}
