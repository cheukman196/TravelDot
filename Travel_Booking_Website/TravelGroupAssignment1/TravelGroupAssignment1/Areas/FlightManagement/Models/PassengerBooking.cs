using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.FlightManagement.Models
{
    public class PassengerBooking
    {

        public FlightBooking FBooking { get; set; }
        public Passenger Passenger { get; set; }


    }
}