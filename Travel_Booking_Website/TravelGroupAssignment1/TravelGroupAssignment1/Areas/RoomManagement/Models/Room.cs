using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Areas.CarManagement.Models;
using TravelGroupAssignment1.Areas.HotelManagement.Models;

namespace TravelGroupAssignment1.Areas.RoomManagement.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [Display(Name = "Room Name")]
        [StringLength(200, ErrorMessage = "Room Name must not exceed 200 characters.")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Room Capacity")]
        [Range(1, 50, ErrorMessage = "Passenger capacity must be between 0 and 50.")]
        public int Capacity { get; set; }

        [Display(Name = "Bed Description")]
        [StringLength(100, ErrorMessage = "Bed description must not exceed 100 characters.")]
        public string? BedDescription { get; set; }

        [Required]
        [Display(Name = "Price Per Night")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
        public double PricePerNight { get; set; }

        [Display(Name = "Room Size")]
        [Range(0, int.MaxValue, ErrorMessage = "Room size must be non-negative.")]
        public int? RoomSize { get; set; }

        // change amenities to class (Amenities:Room = M:M)
        [Display(Name = "Room Amenities")]
        [StringLength(500, ErrorMessage = "Task title must not exceed 500 characters.")]
        public string? Amenities { get; set; }

        [Required]
        public int HotelId { get; set; }

        public Hotel? Hotel { get; set; }

        public ICollection<RoomBooking>? RoomBookings { get; set; }
        public ICollection<RoomComment>? RoomComments { get; set; }

    }
}
