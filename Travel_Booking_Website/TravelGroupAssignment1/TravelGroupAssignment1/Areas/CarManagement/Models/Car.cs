using System.ComponentModel.DataAnnotations;

namespace TravelGroupAssignment1.Areas.CarManagement.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        [Display(Name = "Make")]
        [StringLength(100, ErrorMessage = "Car make must not exceed 100 characters.")]
        public string? Make { get; set; }

        [Required]
        [Display(Name = "Model")]
        [StringLength(100, ErrorMessage = "Car model must not exceed 100 characters.")]
        public string? Model { get; set; }

        [Required]
        [Display(Name = "Type")]
        [StringLength(100, ErrorMessage = "Car type must not exceed 100 characters.")]
        public string? Type { get; set; }

        [Required]
        [Display(Name = "Price Per Day")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public double PricePerDay { get; set; }

        [Display(Name = "Capacity")]
        [Range(1, 1000, ErrorMessage = "Passenger capacity must be between 0 and 1000.")]
        public int MaxPassengers { get; set; }

        [Display(Name = "Transmission")]
        [StringLength(100, ErrorMessage = "Transmission must not exceed 100 characters.")]
        public string? Transmission { get; set; }

        [Display(Name = "Has Air Conditioning")]
        public bool HasAirConditioning { get; set; }

        [Display(Name = "Has Unlimited Mileage")]
        public bool HasUnlimitedMileage { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public CarRentalCompany? Company { get; set; }
        public ICollection<CarBooking>? Bookings { get; set; }
        public ICollection<CarComment>? CarComments { get; set; }
    }

    public enum CarType
    {
        Sedan,
        Hatchback,
        SUV,
        Minivan,
        Van,
        Luxury,
        Sport
    };
}
