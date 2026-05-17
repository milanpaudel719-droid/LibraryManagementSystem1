using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add session service
builder.Services.AddSession();

// Railway MySQL Connection
var connectionString =
    "server=caboose.proxy.rlwy.net;" +
    "port=17114;" +
    "database=railway;" +
    "user=root;" +
    "password=uCNpgbPrESnIzDZmSdeTegvxeRYXXEVr;" +
    "SslMode=Required;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
    ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();