using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Areas.RoomManagement.Models;

namespace TravelGroupAssignment1.Areas.HotelManagement.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        [Display(Name = "Hotel Name")]
        [StringLength(100, ErrorMessage = "Hotel Name must not exceed 100 characters.")]
        public string? HotelName { get; set; }
        
        [Required]
        [Display(Name = "Hotel Location")]
        [StringLength(200, ErrorMessage = "Location must not exceed 200 characters.")]
        public string? Location { get; set; }

        [Display(Name = "Hotel Description")]
        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        // Consider changing Amenities to a separate entity
        // Amentities:Hotel = M:M
        [Display(Name = "Hotel Amenities")]
        [StringLength(1000, ErrorMessage = "Amenities must not exceed 1000 characters.")]
        public string? Amenities { get; set; }

        public ICollection<Room>? Rooms { get; set; }
        public ICollection<HotelComment>? HotelComments { get; set; }
    }
}
