using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using TravelGroupAssignment1.Models;
using TravelGroupAssignment1.Validation;

namespace TravelGroupAssignment1.Areas.CarManagement.Models
{
    public class CarBooking : Booking
    {
        [Required]
        public int CarId { get; set; }
        public Car? Car { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [ValidEndDate("StartDate")] // custom validation tag
        public DateTime? EndDate { get; set; } // validate end date >= start date
        public override string ToEmail()
        {
            string s = base.ToEmail() + "<br>";
            s += $"Details\nMake: {Car.Make} Model: {Car.Model} Capacity: {Car.MaxPassengers} People <br>Price: {Car.PricePerDay} <br>Duration: From {StartDate} to {EndDate}";
            return s;
        }
    }
}
