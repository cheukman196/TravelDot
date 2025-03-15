using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Areas.FlightManagement.Models;

namespace TravelGroupAssignment1.Areas.FlightManagement.Models
{
    public class FlightPassengerAndBooking
    {
        [Required]
        public int FlightId { get; set; }
        public Flight Flight { get; set; }

        [Required]
        public int PassengerId { get; set; }
        public Passenger Passenger { get; set;}

        [Required]
        public int BookingId { get; set; }
        public FlightBooking FlightBooking { get; set; }

        [StringLength(100, ErrorMessage = "Seat must not exceed 100 characters.")]
        public string Seat { get; set; }

    }
}
