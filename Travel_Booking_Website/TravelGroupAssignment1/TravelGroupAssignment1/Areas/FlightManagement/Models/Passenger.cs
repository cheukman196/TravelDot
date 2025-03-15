using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGroupAssignment1.Areas.FlightManagement.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(200, ErrorMessage = "First name must not exceed 200 characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(200, ErrorMessage = "Last name must not exceed 200 characters.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [StringLength(100, ErrorMessage = "Phone number must not exceed 100 characters.")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Passport Number")]
        [StringLength(100, ErrorMessage = "Passport number must not exceed 100 characters.")]
        public string PassportNo { get; set; }

        //public int BookingId { get; set; }
        //public FlightBooking? FlightBooking { get; set; }

    }
}
