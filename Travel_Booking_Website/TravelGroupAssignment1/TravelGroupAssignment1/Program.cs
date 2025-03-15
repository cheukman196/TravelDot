using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Data;
using Microsoft.AspNetCore.Identity;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;
using System;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ** Add DbContext file to the app config **
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// see appsettings.json for default connection string

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
    
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration));
var app = builder.Build();

app.Logger.LogInformation("App building is running");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithRedirects("/Home/NotFound?statusCode={0}");
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");





using var scope = app.Services.CreateScope();
var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

try
{
    //Get servicces needed for role seeding
    //scope.ServiceProvider used to access instances of registered services
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); ;
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); ;
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); ;

    //seed roles
    await ContextSeed.SeedRolesAsync(userManager, roleManager);
    //seed superAdmin
    await ContextSeed.SuperSeedRoleAsync(userManager, roleManager);
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occurred when attempting to seed the roles for the system.");
};



app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseSession();


try
{
    app.Logger.LogInformation("Run Authentication");
    app.UseAuthentication();
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occurred when attempting to Run Authentication.");
}

try
{
    app.Logger.LogInformation("Run Authorization");
    app.UseAuthorization();
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occurred when attempting to Run Authorization.");
}

try
{
    app.Logger.LogInformation("Run MapControlRoute");
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapControllerRoute(
            name: "StatusCode",
            pattern: "StatusCode/{code}",
            defaults: new { controller = "StatusCode", action = "Index" });
    });
    app.MapRazorPages();
    app.Run();
    app.Logger.LogInformation("Starting the app");
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occurred when attempting to Run MapControlRoute.");
}




