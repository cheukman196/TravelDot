using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGroupAssignment1.Models
{
    public class Trip
    {
        [Key]
        public int TripId { get; set; }
        
        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        //public IList<Booking>? Bookings { get; set; }
        [Display(Name = "Trip Reference Number")]
        public string? TripReference { get; set; }

        public Trip()
        {
            TripReference = GenerateTripReference();
        }

        protected virtual String GenerateTripReference()
        {
            string date = DateTime.Now.ToString("yyMMddHHmm");
            string uniqueString = Guid.NewGuid().ToString("").Substring(0, 6);
            return date + uniqueString;
        }

    }
}
