using System.ComponentModel.DataAnnotations;

namespace TravelGroupAssignment1.Areas.CustomerManagement.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

    }
}
