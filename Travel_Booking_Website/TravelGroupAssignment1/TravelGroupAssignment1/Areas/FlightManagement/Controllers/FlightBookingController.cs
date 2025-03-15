using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using TravelGroupAssignment1.Data;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Services;

namespace TravelGroupAssignment1.Areas.FlightManagement.Controllers
{
    [Area("FlightManagement")]
    [Route("[controller]")]
    public class FlightBookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISessionService _sessionService;


        public FlightBookingController(ApplicationDbContext context, ISessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }
        [HttpGet("Index/{flightId:int}")]
        public async Task<IActionResult> Index(int flightId)
        {
            var bookings = await _context.FlightBookings.Where(t => t.FlightId == flightId).Include(t => t.Flight).Include(p => p.Passengers).ToListAsync();

            ViewBag.FlightId = flightId;
            ViewBag.Flight = await _context.Flights.FindAsync(flightId);


            return View(bookings);
        }
        [HttpGet("Create/{flightId:int}")]
        public async Task<IActionResult> Create(int flightId)
        {

            var flight = await _context.Flights.FindAsync(flightId);

            if (flight == null)
            {
                System.Diagnostics.Debug.WriteLine(flightId);

                return NotFound();

            }
            var flightbooking = new FlightBooking
            {
                FlightId = flightId,
            };


            return View(flightbooking);
        }
        [HttpGet("AddPassenger{index:int}")]
        public IActionResult AddPassenger(int index)
        {
            ViewBag["Index"] = index;
            return PartialView("_AddPassenger");
        }

        [HttpPost("CreateBooking")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateBooking([Bind("BookingId, BookingReference, FlightClass, Flight, Seat, FlightId, Passengers")] FlightBooking booking)
        {
            int count = _context.FlightBookings.Where(f => f.FlightId == booking.FlightId).ToList().Count;
            List<int> visitList = _sessionService.GetSessionData<List<int>>("FlightBookingIds");


            if (count >= _context.Flights.Find(booking.FlightId).MaxPassenger)
            {
                return NotFound();
            }
            if (booking.TripId == 0) return View(booking);
            if (ModelState.IsValid)
            {
                int flightId = booking.FlightId;

                if (booking.Passengers.Any())
                {
                    foreach (Passenger p in booking.Passengers)
                    {
                        await _context.Passengers.AddAsync(p);

                    }
                    await _context.SaveChangesAsync();

                }
                else
                {
                    return NotFound();
                }
                await _context.FlightBookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                visitList.Add(booking.BookingId);
                _sessionService.SetSessionData<List<int>>("FlightBookingIds", visitList);
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "FlightBooking", new { flightId = booking.FlightId });
                else
                    return RedirectToAction("Index", "Trip");
            }


            return View(booking);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id, string? con = "FlightBooking")
        {
            var flightbooking = await _context.FlightBookings.Include(t => t.Flight).Include(p => p.Passengers).FirstOrDefaultAsync(booking => booking.BookingId == id);

            // var passengers = _context.Passengers.Where(t => t.Flig == flightbooking.BookingId).ToList();
            if (flightbooking == null) return NotFound();
            //var flightBooking = _context.FlightBookings.Find(id);
            ViewBag.Controller = con;

            return View(flightbooking);

        }
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)

        {
            var booking = await _context.FlightBookings.Include(f => f.Flight).Include(p => p.Passengers).FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();
            var passenger = booking.Passengers[0];

            ViewBag.BookingsList = new SelectList(_context.Flights, "FlightId", "From", booking.FlightId.ToString());

            PassengerBooking passengerBooking = new PassengerBooking
            {
                FBooking = booking,
                Passenger = passenger
            };

            return View(passengerBooking);


        }
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PassengerBooking pbooking)
        {
            Passenger passenger = pbooking.Passenger;
            FlightBooking flightbooking = pbooking.FBooking;
            pbooking.FBooking.Passengers = new List<Passenger> { passenger };
            // System.Diagnostics.Debug.WriteLine("passenger Id is: "+passenger.PassengerId);

            if (id != flightbooking.FlightId)
            {
                return NotFound();

            }
            /*if (ModelState.IsValid)
            {*/
            try
            {
                System.Diagnostics.Debug.WriteLine("Model valid ");

                _context.Passengers.Update(passenger);

                _context.FlightBookings.Update(flightbooking);                 //add new project - only in memory, nothing in database yet
                await _context.SaveChangesAsync(); //commits changes to memory
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                    return RedirectToAction("Index", "FlightBooking", new { flightId = flightbooking.FlightId });
                else
                    return RedirectToAction("Index", "Trip");
            }
            catch (DbUpdateConcurrencyException ex)
            { //for when two updates at the same time-rarely will happen with our form
                if (!FlightBookingExists(flightbooking.BookingId))
                {
                    return NotFound();
                }

                throw;
            }
            //}
            /*System.Diagnostics.Debug.WriteLine(ModelState.ErrorCount);

            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    var errorMessage = error.ErrorMessage;
                    System.Diagnostics.Debug.WriteLine(errorMessage);

                    // Log or handle the error message as needed
                }
            }*/
            return View(pbooking);

        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id, string? con = "FlightBooking")

        {
            var booking = await _context.FlightBookings.Include(t => t.Flight).Include(p => p.Passengers).FirstOrDefaultAsync(b => b.BookingId == id);
            ViewBag.Controller = con;
            if (booking == null) return NotFound();
            return View(booking);


        }
        [HttpPost("DeleteConfirmed/{bookingId:int}"), ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int bookingId, string? con = "FlightBooking")
        {
            List<int> visitList = _sessionService.GetSessionData<List<int>>("FlightBookingIds");

            var booking = _context.FlightBookings.Include(p => p.Passengers).FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null)
            {
                visitList.Remove(bookingId);
                _sessionService.SetSessionData<List<int>>("FlightBookingIds", visitList);

                _context.Passengers.Remove(booking.Passengers[0]);
                _context.FlightBookings.Remove(booking);
                _context.SaveChanges();
                if (string.Equals("FlightBooking", con))
                    return RedirectToAction("Index", con, new { flightId = booking.FlightId });

                return RedirectToAction("Index", con);
            }
            return NotFound();
        }

        public bool FlightBookingExists(int id)
        {

            return _context.FlightBookings.Any(e => e.BookingId == id);
        }


    }
}