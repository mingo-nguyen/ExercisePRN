
using Microsoft.EntityFrameworkCore;
using HoaiNXRazorPages.Infrastructure.Data;
using HoaiNXRazorPages.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HoaiNXRazorPages.Application.ServiceContracts;
using HoaiNXRazorPages.Core.Services;

using HoaiNXRazorPages.Infrastructure.Repositories;
using HoaiNXRazorPages.Domain.RepositoryContracts;
using HoaiNXRazorPages.Application.Services;




var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FUNewsManagementConnection"));
});


builder.Services.AddSignalR();




builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
            policy.RequireClaim(ClaimTypes.Role, "0"));

    options.AddPolicy("RequireStaffRole", policy =>
        policy.RequireClaim(ClaimTypes.Role, "1"));

    options.AddPolicy("RequireLecturerRole", policy =>
        policy.RequireClaim(ClaimTypes.Role, "2"));

    options.AddPolicy("RequireStaffOrAdminRole", policy =>
        policy.RequireClaim(ClaimTypes.Role, "0", "1"));

});

builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();    
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ITagService, TagService>();


builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();

builder.Services.AddScoped<IAuthenticationUserService, AuthenticationUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapHub<NewsHub>("/newsHub");
});

//app.MapFallbackToPage("/Auth/Login");

app.MapRazorPages();

app.Run();