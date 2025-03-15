using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Models;

namespace TravelGroupAssignment1.Areas.CarManagement.Models
{
    public class CarComment : Comment 
    {
        [Required]
        public int CarId {  get; set; }

        public Car? Car { get; set; }
    }
}
