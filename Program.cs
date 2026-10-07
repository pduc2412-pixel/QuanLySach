using Microsoft.EntityFrameworkCore;
using QuanLySach.Data;
using QuanLySach.Middlewares;

Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"));
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Sach/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sach}/{action=Index}/{id?}");

app.Run();
