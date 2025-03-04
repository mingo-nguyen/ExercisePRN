using BlazorWebAppMovies.Components;
using BlazorWebAppMovies.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebAppMovies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Add DbContext
            builder.Services.AddDbContext<BlazorWebAppMoviesContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MovieContext")));

            builder.Services.AddQuickGridEntityFrameworkAdapter();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register IDbContextFactory with transient lifetime
            builder.Services.AddDbContextFactory<BlazorWebAppMoviesContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MovieContext")), ServiceLifetime.Transient);

            var app = builder.Build();

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

            app.Run();
        }
    }
}
