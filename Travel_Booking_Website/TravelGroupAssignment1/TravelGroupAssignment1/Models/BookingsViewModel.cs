using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.RoomManagement.Models;
using TravelGroupAssignment1.Areas.FlightManagement.Models;

namespace TravelGroupAssignment1.Models
{
    public class BookingsViewModel
    {
        public ICollection<FlightBooking> flights {  get; set; }
        public ICollection<CarBooking> cars { get; set; }
        public ICollection<RoomBooking> rooms { get; set; }
    }
}
