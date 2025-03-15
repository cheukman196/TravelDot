using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.RoomManagement.Models
{
    public class RoomComment : Comment
    {
        [Required]
        public int RoomId { get; set; }

        public Room? Room { get; set; }
    }
}
