using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Services;

namespace TravelGroupAssignment1.Areas.CarManagement.Controllers
{
    [Area("CarManagement")]
    [Route("[controller]")]
    public class CarBookingController : Controller
    {
        // required
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;


        // required for DI 
        public CarBookingController(ApplicationDbContext context, ISessionService sessionservice)
        {
            _context = context;
            _sessionService = sessionservice;
        }

        // GET: CarBookingController/5
        [HttpGet("Index/{carId:int}")]
        public async Task<IActionResult> Index(int carId)
        {
            var bookings = await _context.CarBookings
                        .Include(c => c.Car)
                        .Where(c => c.CarId == carId)
                        .ToListAsync();
            if (bookings == null) return NotFound();
            Car? car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.CarId = carId;
            return View(bookings);
        }

        // GET: CarBookingController/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id, string? con = "CarBooking")
        {
            var booking = await _context.CarBookings
                        .Include(cb => cb.Car)
                        .ThenInclude(c => c.Company)
                        .FirstOrDefaultAsync(cb => cb.BookingId == id);
            if (booking == null) return NotFound();
            ViewBag.Controller = con;
            return View(booking);
        }

        [HttpGet("Create/{carId:int}/{startDate:datetime}/{endDate:datetime}")]
        public async Task<IActionResult> Create(int carId, DateTime? startDate, DateTime? endDate)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();
            var company = await _context.CarRentalCompanies.FindAsync(car.CompanyId);
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.Car = car;
            ViewBag.Company = company;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(new CarBooking { CarId = car.CarId });
        }

        [HttpGet("Create/{carId:int}")]
        public async Task<IActionResult> Create(int carId)
        {

            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();
            var company = await _context.CarRentalCompanies.FindAsync(car.CompanyId);
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.Car = car;
            ViewBag.Company = company;
            return View(new CarBooking { CarId = car.CarId, TripId = 1 });
        }

        // POST: CarBookingController/Create
        [HttpPost("CreateBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking([Bind("BookingReference",
            "CarId", "Car", "StartDate", "EndDate")] CarBooking carBooking)
        {
            List<int> visitList = _sessionService.GetSessionData<List<int>>("CarBookingIds");

            // View bag components to be show in page
            var car = await _context.Cars.FindAsync(carBooking.CarId);
            if (car == null) return NotFound();
            var company = await _context.CarRentalCompanies.FindAsync(car.CompanyId);
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.Car = car;
            ViewBag.Company = company;

            if (ModelState.IsValid)
            {
                // check if car is booked 
                if (await carBookingExists(carBooking))
                {
                    ModelState.AddModelError("", "Car is not available for booking on given date range.");
                    return View("Create", carBooking);
                }
                await _context.CarBookings.AddAsync(carBooking);
                await _context.SaveChangesAsync();
                visitList.Add(carBooking.BookingId);
                _sessionService.SetSessionData<List<int>>("CarBookingIds", visitList);

                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "CarBooking", new { carId = carBooking.CarId });
                else
                    return RedirectToAction("Index", "Trip");
            }
            return View("Create", carBooking);
        }

        // GET: CarBookingController/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var carBooking = await _context.CarBookings
                            .Include(cb => cb.Car)
                            .FirstOrDefaultAsync(cb => cb.BookingId == id);
            if (carBooking == null) return NotFound();



            var car = await _context.Cars.FindAsync(carBooking.CarId);
            if (car == null) return NotFound();
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.Car = car;

            return View(carBooking);
        }

        // POST: CarBookingController/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId", "TripId", "BookingReference",
            "CarId", "Car", "StartDate", "EndDate")] CarBooking carBooking)
        {
            if (id != carBooking.BookingId) return NotFound();

            var car = await _context.Cars.FindAsync(carBooking.CarId);
            if (car == null) return NotFound();
            ViewBag.CarName = car.Make + " " + car.Model;
            ViewBag.CarType = car.Type;
            ViewBag.Car = car;

            if (ModelState.IsValid)
            {
                if (await carBookingExists(carBooking))
                {
                    ModelState.AddModelError("", "Car is not available for booking on given date range.");
                    return View(carBooking);
                }
                _context.CarBookings.Update(carBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { carId = carBooking.CarId });
            }
            return View(carBooking);

        }

        // GET: CarBookingController/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id, string? con = "CarBooking")
        {
            var carBooking = await _context.CarBookings
                        .Include(cb => cb.Car)
                        .FirstOrDefaultAsync(cb => cb.BookingId == id);
            if (carBooking == null) return NotFound();
            ViewBag.Controller = con;
            System.Diagnostics.Debug.WriteLine(con);
            return View(carBooking);
        }

        // POST: CarBookingController/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? con = "Trip")
        {
            List<int> visitList = _sessionService.GetSessionData<List<int>>("CarBookingIds");

            var carBooking = await _context.CarBookings.FindAsync(id);


            if (carBooking != null)
            {
                _context.CarBookings.Remove(carBooking);
                await _context.SaveChangesAsync();
                visitList.Remove(carBooking.BookingId);
                _sessionService.SetSessionData<List<int>>("CarBookingIds", visitList);

                if (string.Equals(con, "Trip"))
                    return RedirectToAction("Index", con);
                return RedirectToAction("Index", new { carId = carBooking.CarId });
            }
            return NotFound();
        }

        // Helper function: check if no identical carbooking (car, start date, end date) exists
        public async Task<bool> carBookingExists(CarBooking carBooking)
        {
            var carBookingQuery = from p in _context.CarBookings
                                  select p;
            carBookingQuery = carBookingQuery.Where(c => c.CarId == carBooking.CarId)
                                            .Where(c => c.StartDate >= carBooking.StartDate && c.StartDate <= carBooking.EndDate ||
                                            c.EndDate >= carBooking.StartDate && c.EndDate <= carBooking.EndDate);
            var existingCarBookings = await carBookingQuery.ToListAsync();
            return existingCarBookings.Any();
        }
    }
}