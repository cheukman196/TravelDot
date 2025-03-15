using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using Microsoft.AspNetCore.Authorization;
using TravelGroupAssignment1.Controllers;

namespace TravelGroupAssignment1.Areas.FlightManagement.Controllers
{
    [Area("FlightManagement")]
    [Route("[controller]")]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FlightController> _logger;

        public FlightController(ApplicationDbContext context, ILogger<FlightController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Calling Flight Index() Action");
            try
            {
                var flights = await _context.Flights.ToListAsync();
                return View(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Calling Flight Create() Get Action");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flight newFlight)
        {
            _logger.LogInformation("Calling Flight Create() Post Action");
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.Flights.AddAsync(newFlight);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(newFlight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("Details/{flightId:int}")]
        public async Task<IActionResult> Details(int flightId)
        {
            _logger.LogInformation("Calling Flight Details() Action");
            try
            {
                var flight = await _context.Flights.FindAsync(flightId);
                return View(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("Edit/{flightId:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Edit(int flightId)
        {
            _logger.LogInformation("Calling Flight Edit() Get Action");
            try
            {
                var flight = _context.Flights.Find(flightId);
                return View(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpPost("Edit/{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("FlightId", "Airline", "Price", "MaxPassenger", "From", "To", "DepartTime", "ArrivalTime")] Flight flight)
        {
            _logger.LogInformation("Calling Flight Edit() Post Action");
            try
            {
                if (id != flight.FlightId)
                {
                    System.Diagnostics.Debug.WriteLine(id);
                    System.Diagnostics.Debug.WriteLine(flight.FlightId);
                    Console.WriteLine(flight.FlightId);
                    // return RedirectToAction("Index", "Flight", id, flight.FlightId);
                    return RedirectToAction("Index");

                }
                if (ModelState.IsValid)
                {
                    _context.Flights.Update(flight);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("Delete/{flightId:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public IActionResult Delete(int flightId)
        {
            _logger.LogInformation("Calling Flight Delete() Get Action");
            try
            {
                var flight = _context.Flights.Find(flightId);
                if (flight == null) return NotFound();
                return View(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpPost("DeleteConfirmed/{flightId:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int flightId)
        {
            _logger.LogInformation("Calling Flight DeleteConfirmed() Post Action");
            try
            {
                var flight = await _context.Flights.FindAsync(flightId);
                if (flight != null)
                {
                    _context.Remove(flight);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        public bool FlightExists(int id)
        {

            return _context.Flights.Any(e => e.FlightId == id);
        }

        [HttpGet("Search/{searchString?}")]
        public async Task<IActionResult> Search(string locationFrom, string location, int capacity, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Flight Search: {locationFrom}, {location}, {capacity}, {startDate}, {endDate}", locationFrom, location, capacity, startDate, endDate);
            try
            {
                var flightQuery = from p in _context.Flights
                                  select p;

                bool searchValid = !string.IsNullOrEmpty(location) && capacity > 0;
                if (searchValid)
                {
                    flightQuery = flightQuery.Where(f => f.From.Contains(locationFrom) && f.To.Contains(location));
                    // I changed line below, not sure if I fixed it or not
                    flightQuery = flightQuery.Where(f => f.DepartTime.Date >= startDate.Date && f.ArrivalTime.Date <= endDate.Date);

                }
                else
                {
                    return RedirectToAction("Index");
                }
                var flights = await flightQuery.ToListAsync();
                ViewBag.SearchValid = searchValid;
                ViewBag.Location = location;
                ViewBag.Capacity = capacity;
                ViewBag.StartDate = startDate;
                return View("Index", flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("SearchAjax/{searchString?}")]
        public async Task<IActionResult> SearchAjax(string locationFrom, string location, int capacity, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Flight Search: {locationFrom}, {location}, {capacity}, {startDate}, {endDate}", locationFrom, location, capacity, startDate, endDate);
            try
            {
                var flightQuery = from p in _context.Flights
                                  select p;

                bool searchValid = !string.IsNullOrEmpty(location) && capacity > 0;
                if (searchValid)
                {
                    flightQuery = flightQuery.Where(f => f.From.Contains(locationFrom) && f.To.Contains(location));
                    // I changed line below, not sure if I fixed it or not
                    flightQuery = flightQuery.Where(f => f.DepartTime.Date >= startDate.Date && f.ArrivalTime.Date <= endDate.Date);

                }
                else
                {
                    return RedirectToAction("Index");
                }
                var flights = await flightQuery.ToListAsync();

                return Json(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }



    }
}
