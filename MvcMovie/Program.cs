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

// Seed the database if applicable
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
