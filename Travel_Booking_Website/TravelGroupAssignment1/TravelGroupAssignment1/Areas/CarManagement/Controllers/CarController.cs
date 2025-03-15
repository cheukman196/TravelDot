using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Controllers;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.CarManagement.Controllers
{
    [Area("CarManagement")]
    [Route("[controller]")]
    public class CarController : Controller
    {
        // required
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarController> _logger;

        // required for DI 
        public CarController(ApplicationDbContext context, ILogger<CarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: CarController
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Calling Car Index() Action");
            try
            {
                var cars = await _context.Cars
                    .Include(cars => cars.Company)
                    .ToListAsync();
                if (cars == null) return NotFound();
                return View(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }

        // GET: CarController/Details/5
        [HttpGet("Details/{carId:int}")]
        public async Task<IActionResult> Details(int carId)
        {
            _logger.LogInformation("Calling Car Details() Action");
            try
            {
                var car = await _context.Cars
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(c => c.CarId == carId);
                        if (car == null) return NotFound();
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }


        // GET: CarController/Create
        [HttpGet("Create")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Calling Car Create() Get Action");
            try
            {
                ViewBag.Companies = new SelectList(_context.CarRentalCompanies, "CarRentalCompanyId", "CompanyName");
                var companyObjects = await _context.CarRentalCompanies
                                            .ToDictionaryAsync(c => c.CarRentalCompanyId, c => c);
                ViewBag.CompanyObjects = companyObjects;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // POST: CarController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Create([Bind("Make", "Model", "Type", "PricePerDay", "MaxPassengers",
            "CompanyId", "Company", "Transmission", "HasAirConditioning", "HasUnlimitedMileage")] Car car)
        {
            _logger.LogInformation("Calling Car Create() Post Action");
            try
            {
                if (ModelState.IsValid)
                {
                    // retrieve Company navigation property via id
                    CarRentalCompany? company = await _context.CarRentalCompanies.FirstOrDefaultAsync(cr => cr.CarRentalCompanyId == car.CompanyId);
                    // create new Car object with navigation property
                    Car newCar = new Car
                    {
                        Make = car.Make,
                        Model = car.Model,
                        Type = car.Type,
                        PricePerDay = car.PricePerDay,
                        MaxPassengers = car.MaxPassengers,
                        Transmission = car.Transmission,
                        HasAirConditioning = car.HasAirConditioning,
                        HasUnlimitedMileage = car.HasUnlimitedMileage,
                        CompanyId = car.CompanyId,
                        Company = company
                    };
                    await _context.Cars.AddAsync(newCar);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.Companies = new SelectList(_context.CarRentalCompanies, "CarRentalCompanyId", "CompanyName", car.CarId);
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // GET: CarController/Edit/5
        [HttpGet("Edit/{carId:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int carId)
        {
            _logger.LogInformation("Calling Car Edit() Get Action");
            try
            {
                var car = await _context.Cars
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null) return NotFound();

                ViewBag.CompanyList = new SelectList(_context.CarRentalCompanies, "CarRentalCompanyId", "CompanyName", car.CarId);
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }

        }

        // POST: CarController/Edit/5
        [HttpPost("Edit/{carId:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Edit(int carId, [Bind("CarId", "Make", "Model", "Type", "PricePerDay", "MaxPassengers",
            "CompanyId", "Company", "Transmission", "HasAirConditioning", "HasUnlimitedMileage")] Car car)
        {
            _logger.LogInformation("Calling Car Edit() Post Action");
            try
            {
                if (carId != car.CarId) return NotFound();

                if (ModelState.IsValid)
                {
                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.CompanyList = new SelectList(_context.CarRentalCompanies, "CompanyId", "CompanyName");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // GET: CarController/Delete/5
        [HttpGet("Delete/{carId:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> Delete(int carId)
        {
            _logger.LogInformation("Calling Car delete() Get Action");
            try
            {
                var car = await _context.Cars
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(c => c.CarId == carId);
                if (car == null) return NotFound();
                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // POST: CarController/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{carId:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int carId)
        {
            _logger.LogInformation("Calling Car delete() Post Action");
            try
            {
                var car = await _context.Cars.FindAsync(carId);
                if (car != null)
                {
                    _context.Cars.Remove(car);
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

        [HttpGet("Search/{searchString?}")]
        public async Task<IActionResult> Search(string location, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Requested Search at for {location} with {startDate} and {endDate}", location, startDate, endDate);
            try
            {
                var carQuery = from p in _context.Cars
                               select p;
                bool searchValid = !string.IsNullOrEmpty(location);
                if (searchValid)
                {
                    // search by location, and by cars with no bookings in given date range
                    carQuery = carQuery.Where(c => c.Company != null && c.Company.Location.Contains(location))
                                    .Where(c => !c.Bookings.Any(b => b.EndDate >= startDate && b.EndDate <= endDate
                                        || b.StartDate >= startDate && b.StartDate <= endDate));
                }
                else
                {
                    return RedirectToAction("Index");
                }
                var cars = await carQuery.Include(c => c.Company).ToListAsync();
                ViewBag.SearchValid = searchValid;
                ViewBag.Location = location;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View("Index", cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("SearchAjax/{searchString?}")]
        public async Task<IActionResult> SearchAjax(string location, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Requested Search at for {location} with {startDate} and {endDate}", location, startDate, endDate);
            try
            {
                var carQuery = from p in _context.Cars
                               select p;
                bool searchValid = !string.IsNullOrEmpty(location);
                if (searchValid)
                {
                    // search by location, and by cars with no bookings in given date range
                    carQuery = carQuery.Where(c => c.Company != null && c.Company.Location.Contains(location))
                                    .Where(c => !c.Bookings.Any(b => b.EndDate >= startDate && b.EndDate <= endDate
                                        || b.StartDate >= startDate && b.StartDate <= endDate));

                    var cars = await carQuery.Include(c => c.Company).Select(c => new
                    {
                        carId = c.CarId,
                        make = c.Make,
                        model = c.Model,
                        maxPassengers = c.MaxPassengers,
                        transmission = c.Transmission,
                        hasAirConditioning = c.HasAirConditioning,
                        hasUnlimitedMileage = c.HasUnlimitedMileage,
                        pricePerDay = c.PricePerDay,
                        companyName = c.Company.CompanyName
                    }).ToListAsync();


                    return Json(cars);
                }
                else
                {
                    return StatusCode(400, "Search invalid");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }
    }
}
