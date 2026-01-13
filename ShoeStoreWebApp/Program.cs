using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoeStoreWebApp.Data;


var builder = WebApplication.CreateBuilder(args);

// =======================
// Add services to the container
// =======================

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // ⭐ Đây là phần còn thiếu


// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSession();

var app = builder.Build();

// =======================
// Configure the HTTP request pipeline
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

// ROUTE AREA (PHẢI ĐẶT TRƯỚC)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// ROUTE MẶC ĐỊNH
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // ⭐ Quan trọng để Identity UI hoạt động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ShoeStoreWebApp.Data.DbInitializer.SeedRolesAndAdminAsync(services);
}
app.MapAreaControllerRoute(
    name: "Admin_default",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.Run();
