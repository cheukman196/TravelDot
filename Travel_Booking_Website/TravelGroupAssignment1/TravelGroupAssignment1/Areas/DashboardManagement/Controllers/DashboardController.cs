using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Controllers;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.DashboardManagement.Models.ViewModels;
using TravelGroupAssignment1.Areas.HotelManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Controllers;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.DashboardManagement.Controllers
{
    [Area("DashboardManagement")]
    [Route("[area]/[controller]")]
    /*[Authorize(Roles = "SuperAdmin, Admin")]*/
    public class DashboardController : Controller
    {
        // required
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        // required for DI 
        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Calling Dashboard Index() Action");
            try
            {
                var model = new DashboardViewModel();
                // Assuming you have DbContext set up
                model.HotelCount = await _context.Hotels.CountAsync();
                model.RoomCount = await _context.Rooms.CountAsync();
                model.FlightCount = await _context.Flights.CountAsync();
                model.CarRentalCompanyCount = await _context.CarRentalCompanies.CountAsync();
                model.CarCount = await _context.Cars.CountAsync();
                model.UserCount = await _context.Users.CountAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }
    }
}

    