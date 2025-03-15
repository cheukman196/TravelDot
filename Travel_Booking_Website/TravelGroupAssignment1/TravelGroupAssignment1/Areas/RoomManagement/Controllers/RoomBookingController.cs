using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Services;

namespace TravelGroupAssignment1.Areas.RoomManagement.Controllers
{
    [Area("RoomManagement")]
    [Route("[controller]")]
    public class RoomBookingController : Controller
    {
        // required
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;


        // required for DI 
        public RoomBookingController(ApplicationDbContext context, ISessionService sessionservice)
        {
            _context = context;
            _sessionService = sessionservice;
        }

        // GET: RoomBookingController/5
        [HttpGet("Index/{roomId:int}")]
        public async Task<IActionResult> Index(int roomId)
        {
            var roomBookings = await _context.RoomBookings
                                .Include(rb => rb.Room)
                                .Where(rb => rb.RoomId == roomId)
                                .ToListAsync();

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId);
            if (room == null) return NotFound();
            ViewBag.RoomName = room.Name;
            ViewBag.RoomId = room.RoomId;
            ViewBag.HotelId = room.HotelId;
            return View(roomBookings);
        }

        // GET: RoomBookingControllers/Details/5
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id, string? con = "RoomBooking")
        {
            var booking = await _context.RoomBookings
                        .Include(rb => rb.Room)
                        .FirstOrDefaultAsync(rb => rb.BookingId == id);
            if (booking == null) return NotFound();
            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();
            ViewBag.Hotel = hotel;
            ViewBag.Controller = con;
            return View(booking);
        }

        // GET: RoomBookingController/Create/5
        [HttpGet("Create/{roomId:int}/{checkInDate:datetime}/{checkOutDate:datetime}")]
        public async Task<IActionResult> Create(int roomId, DateTime? checkInDate, DateTime? checkOutDate)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();

            ViewBag.Room = room;
            ViewBag.Hotel = hotel;

            ViewBag.CheckInDate = checkInDate;
            ViewBag.CheckOutDate = checkOutDate;

            return View(new RoomBooking { RoomId = roomId, TripId = 1 });
        }

        // Create for admins
        [HttpGet("Create/{roomId:int}")]
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();

            ViewBag.Room = room;
            ViewBag.Hotel = hotel;

            return View(new RoomBooking { RoomId = roomId, TripId = 1, CheckInDate = DateTime.Now, CheckOutDate = DateTime.Now.AddDays(2) });
        }

        // POST: RoomBookingController/CreateBooking
        [HttpPost("CreateBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking([Bind("TripId", "BookingReference",
            "RoomId", "Room", "CheckInDate", "CheckOutDate")] RoomBooking roomBooking)
        {
            List<int> visitList = _sessionService.GetSessionData<List<int>>("RoomBookingIds");

            // information needed if booking not successfull created
            var room = await _context.Rooms.FindAsync(roomBooking.RoomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();
            ViewBag.Room = room;
            ViewBag.Hotel = hotel;

            if (ModelState.IsValid)
            {
                // check if room is already booked on given dates
                if (await roomBookingExists(roomBooking))
                {
                    ModelState.AddModelError("", "Room is not available for booking on given date range.");
                    return View("Create", roomBooking);
                }

                await _context.RoomBookings.AddAsync(roomBooking);
                await _context.SaveChangesAsync();
                visitList.Add(roomBooking.BookingId);
                _sessionService.SetSessionData<List<int>>("RoomBookingIds", visitList);

                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "RoomBooking", new { roomId = roomBooking.RoomId });
                else
                    return RedirectToAction("Index", "Trip");
            }
            return View("Create", roomBooking);
        }

        // GET: RoomBookingController/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var roomBooking = await _context.RoomBookings
                            .Include(rb => rb.Room)
                            .FirstOrDefaultAsync(cb => cb.BookingId == id);
            if (roomBooking == null) return NotFound();
            var room = await _context.Rooms.FindAsync(roomBooking.RoomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();
            if (roomBooking == null) return NotFound();
            ViewBag.Room = room;
            ViewBag.Hotel = hotel;
            return View(roomBooking);
        }

        // POST: RoomBookingController/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId", "TripId", "BookingReference",
            "RoomId", "Room", "CheckInDate", "CheckOutDate")] RoomBooking roomBooking)
        {
            if (id != roomBooking.BookingId) return NotFound();

            // info to render view if failure
            var room = await _context.Rooms.FindAsync(roomBooking.RoomId);
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (roomBooking == null) return NotFound();
            ViewBag.Room = room;
            ViewBag.Hotel = hotel;

            if (ModelState.IsValid)
            {
                if (await roomBookingExists(roomBooking))
                {
                    ModelState.AddModelError("", "Room is not available for booking on given date range.");
                    return View(roomBooking);
                }
                _context.RoomBookings.Update(roomBooking);
                await _context.SaveChangesAsync();

                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "RoomBooking", new { roomId = roomBooking.RoomId });
                else
                    return RedirectToAction("Index", "Trip");
            }
            return View(roomBooking);
        }

        // GET: RoomBookingController/Delete/5
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id, string? con = "RoomBooking")
        {
            var booking = await _context.RoomBookings
                        .Include(rb => rb.Room)
                        .FirstOrDefaultAsync(rb => rb.BookingId == id);
            if (booking == null) return NotFound();
            var room = await _context.Rooms.FindAsync(booking.RoomId);
            if (room == null) return NotFound();
            var hotel = await _context.Hotels.FindAsync(room.HotelId);
            if (hotel == null) return NotFound();
            ViewBag.Hotel = hotel;
            ViewBag.Controller = con;

            return View(booking);
        }

        // POST: RoomBookingController/DeleteConfirmed/5
        [HttpPost("DeleteConfirmed/{id:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? con = "Trip")
        {
            var roomBooking = await _context.RoomBookings.FindAsync(id);
            List<int> visitList = _sessionService.GetSessionData<List<int>>("RoomBookingIds");

            if (roomBooking != null)
            {
                visitList.Remove(id);
                _sessionService.SetSessionData<List<int>>("RoomBookingIds", visitList);

                _context.RoomBookings.Remove(roomBooking);
                await _context.SaveChangesAsync();
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "RoomBooking", new { roomId = roomBooking.RoomId });
                else
                    return RedirectToAction("Index", "Trip");
            }
            return NotFound();
        }

        public async Task<bool> roomBookingExists(RoomBooking roomBooking)
        {
            var roomBookingQuery = from p in _context.RoomBookings
                                   select p;
            roomBookingQuery = roomBookingQuery.Where(r => r.RoomId == roomBooking.RoomId)
                                            .Where(r => r.CheckInDate >= roomBooking.CheckInDate && r.CheckInDate <= roomBooking.CheckOutDate ||
                                            r.CheckOutDate >= roomBooking.CheckInDate && r.CheckOutDate <= roomBooking.CheckOutDate);
            var existingRoomBookings = await roomBookingQuery.ToListAsync();
            return existingRoomBookings.Count > 0;
        }
    }
}