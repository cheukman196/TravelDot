using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.HotelManagement.Models;
using TravelGroupAssignment1.Controllers;
using TravelGroupAssignment1.Data;

namespace TravelGroupAssignment1.Areas.HotelManagement.Controllers
{
    [Area("HotelManagement")]
    [Route("[controller]")]
    public class HotelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HotelController> _logger;

        public HotelController(ApplicationDbContext context, ILogger<HotelController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: HotelController
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Calling Hotel Index() Action");
            try
            {
                var hotels = await _context.Hotels.ToListAsync();
                return View(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // GET: HotelController/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Calling Hotel Details() Action");
            try
            {
                var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == id);
                if (hotel == null) return NotFound();
                return View(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }
        // GET: HotelController/Create

        [HttpGet("Create")]
        public IActionResult Create()
        {
            _logger.LogInformation("Calling Hotel Create() Get Action");
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

        // POST: HotelController/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel newHotel)
        {
            _logger.LogInformation("Calling Hotel Create() Post Action");
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.Hotels.AddAsync(newHotel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(newHotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // GET: HotelController/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Calling Hotel Edit() Get Action");
            try
            {
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel == null) return NotFound();
                return View(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // POST: HotelController/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HotelId", "HotelName", "Location", "Description", "Amenities")] Hotel hotel)
        {
            _logger.LogInformation("Calling Hotel Edit() Post Action");
            try
            {
                if (id != hotel.HotelId) return NotFound();
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Hotels.Update(hotel);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await HotelExists(id)) return NotFound();
                        else throw;
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // GET: HotelController/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Calling Hotel Delete() Get Action");
            try
            {
                var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == id);
                if (hotel == null) return NotFound();
                return View(hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        // POST: HotelController/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Calling Hotel Delete() Post Action");
            try
            {
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel != null)
                {
                    _context.Remove(hotel);
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
        public async Task<IActionResult> Search(string location, int capacity, DateTime checkInDate, DateTime checkOutDate)
        {
            _logger.LogInformation("Hotel Search: {location}, {capacity}, {checkInDate}, {checkOutDate}", location, capacity, checkInDate, checkOutDate);
            try
            {
                var hotelQuery = from p in _context.Hotels
                                 select p;
                bool searchValid = !string.IsNullOrEmpty(location) && capacity > 0;
                if (searchValid)
                {
                    hotelQuery = hotelQuery.Where(h => !string.IsNullOrEmpty(h.Location) && h.Location.Contains(location) ||
                                            !string.IsNullOrEmpty(h.Description) && h.Description.Contains(location));
                    hotelQuery = hotelQuery.Where(h => h.Rooms != null && h.Rooms.Any(r => r.Capacity >= capacity &&
                                            !r.RoomBookings.Any(rb => checkOutDate >= rb.CheckInDate && checkInDate <= rb.CheckOutDate)));

                }
                else
                {
                    return RedirectToAction("Index");
                }
                var hotels = await hotelQuery.ToListAsync();
                ViewBag.SearchValid = searchValid;
                ViewBag.Location = location;
                ViewBag.Capacity = capacity;
                ViewBag.CheckInDate = checkInDate;
                ViewBag.CheckOutDate = checkOutDate;
                return View("Index", hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        [HttpGet("SearchAjax/{searchString?}")]
        public async Task<IActionResult> SearchAjax(string location, int capacity, DateTime checkInDate, DateTime checkOutDate)
        {
            _logger.LogInformation("Hotel Search: {location}, {capacity}, {checkInDate}, {checkOutDate}", location, capacity, checkInDate, checkOutDate);
            try
            {
                var hotelQuery = from p in _context.Hotels
                                 select p;
                bool searchValid = !string.IsNullOrEmpty(location) && capacity > 0;
                if (searchValid)
                {
                    hotelQuery = hotelQuery.Where(h => !string.IsNullOrEmpty(h.Location) && h.Location.Contains(location) ||
                                            !string.IsNullOrEmpty(h.Description) && h.Description.Contains(location));
                    hotelQuery = hotelQuery.Where(h => h.Rooms != null && h.Rooms.Any(r => r.Capacity >= capacity &&
                                            !r.RoomBookings.Any(rb => checkOutDate >= rb.CheckInDate && checkInDate <= rb.CheckOutDate)));
                }
                else
                {
                    return RedirectToAction("Index");
                }
                var hotels = await hotelQuery.ToListAsync();

                return Json(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View(null);
            }
        }

        public async Task<bool> HotelExists(int id)
        {
            return await _context.Hotels.AnyAsync(h => h.HotelId == id);
        }
    }
}
