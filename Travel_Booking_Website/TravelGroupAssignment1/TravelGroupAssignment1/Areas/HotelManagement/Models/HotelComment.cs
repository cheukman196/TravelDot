using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.HotelManagement.Models
{
    public class HotelComment : Comment
    {
        [Required]
        public int HotelId { get; set; }

        public Hotel? Hotel { get; set; }
    }
}
