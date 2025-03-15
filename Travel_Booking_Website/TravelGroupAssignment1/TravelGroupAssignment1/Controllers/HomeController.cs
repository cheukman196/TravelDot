using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Services;

namespace TravelGroupAssignment1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ISessionService sessionService)
        {
            _logger = logger;
            _context = context;
            _sessionService = sessionService;
            _sessionService.SetSessionData<List<int>>("FlightBookingIds", new List<int>());
            _sessionService.SetSessionData<List<int>>("CarBookingIds", new List<int>());
            _sessionService.SetSessionData<List<int>>("RoomBookingIds", new List<int>());

        }

        public IActionResult Index()
        {

            _logger.LogInformation("Calling Home Index() Action");
            try
            {

                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                {
                    // Redirect to specific page for SuperAdmin or Admin
                    return RedirectToAction("Index", "Dashboard", new { area = "DashboardManagement" });
                }

                // Proceed with normal behavior if not SuperAdmin or Admin
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }
        

        public IActionResult LoadPartialView()
        {
            return PartialView("_PartialView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
