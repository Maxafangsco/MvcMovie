using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using MvcMovie.Models;

var builder = WebApplication.CreateBuilder(args);

// Retrieve connection string or use an in-memory database as fallback
string connectionString = builder.Configuration.GetConnectionString("MvcMovieContext");
if (string.IsNullOrEmpty(connectionString))
{
    // Use in-memory database if no connection string is provided
    builder.Services.AddDbContext<MvcMovieContext>(options =>
        options.UseInMemoryDatabase("MvcMovieInMemoryDb"));
    
    Console.WriteLine("Warning: Using in-memory database because connection string was not found.");
}
else
{
    // Use SQL Server with the provided connection string
    builder.Services.AddDbContext<MvcMovieContext>(options =>
        options.UseSqlServer(connectionString));
}

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
