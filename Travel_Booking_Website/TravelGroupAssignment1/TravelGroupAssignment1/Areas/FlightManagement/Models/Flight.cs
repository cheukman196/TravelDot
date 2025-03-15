using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TravelGroupAssignment1.Areas.FlightManagement.Models;
using TravelGroupAssignment1.Validation;

namespace TravelGroupAssignment1.Areas.FlightManagement.Models
{
    public class Flight
    {

        [Key]  
        public int FlightId { get; set; }

        [Required]
        [Display(Name = "Airline")]
        [StringLength(200, ErrorMessage = "Airline name must not exceed 200 characters.")]
        public string Airline { get; set;}

        [Required]
        [Display(Name = "Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public double Price { get; set;}

        [Required]
        [Display(Name = "Plane Capacity")]
        [Range(1, 5000, ErrorMessage = "Plane Capacity must be between 1 and 5000.")]
        public int MaxPassenger { get; set;}

        public IEnumerable<Passenger>? PassengerList { get; set;}

        [Required]
        [Display(Name = "Flight Origin")]
        [StringLength(200, ErrorMessage = "Flight origin must not exceed 200 characters.")]
        public string From { get; set;}

        [Required]
        [Display(Name = "Flight Destination")]
        [StringLength(200, ErrorMessage = "Flight destination must not exceed 200 characters.")]
        public string To { get; set;}

        [Required]
        [DisallowNull]
        [DataType(DataType.DateTime)]
        [Display(Name = "Flight Depart Time")]
        public DateTime DepartTime { get; set;}

        [Required]
        [DisallowNull]
        [DataType(DataType.DateTime)]
        [ValidEndDate("DepartTime")]
        [Display(Name = "Flight Arrival Time")]
        public DateTime ArrivalTime { get; set; }



    }
}
