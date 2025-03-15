using System.ComponentModel.DataAnnotations;

namespace TravelGroupAssignment1.Areas.CarManagement.Models
{
    public class CarRentalCompany
    {
        [Key]
        public int CarRentalCompanyId { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [StringLength(200, ErrorMessage = "Company name must not exceed 200 characters.")]
        public string? CompanyName { get; set; }

        [Required]
        [Display(Name = "Location")]
        [StringLength(200, ErrorMessage = "Location must not exceed 200 characters.")]
        public string? Location { get; set; }

        [Display(Name = "Rating")]
        [Range(0.00, 5.00, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        public ICollection<Car>? Cars { get; set; }

    }
}
