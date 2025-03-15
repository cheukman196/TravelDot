using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.FlightManagement.Models
{
    public class FlightBooking : Booking
    {

        [ForeignKey("Flight")]
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }


        [Required]
        [Display(Name = "Flight Class")]
        [StringLength(100, ErrorMessage = "Flight class must not exceed 100 characters.")]
        public string FlightClass { get; set; }

        [StringLength(100, ErrorMessage = "Seat must not exceed 100 characters.")]
        public string Seat { get; set; }

        public IList<Passenger> Passengers { get; set; }

        // public Passenger[]? Passengers { get; set; } = null;
        public override string ToEmail()
        {
            string s = base.ToEmail() + "<br>";
            s += $"From: {Flight.From}  To: {Flight.To} <br>Departure: {Flight.DepartTime} Arrival: {Flight.ArrivalTime} <br>Seat Class: {FlightClass}   Seat: {Seat}";
            return s;
        }
    }
}