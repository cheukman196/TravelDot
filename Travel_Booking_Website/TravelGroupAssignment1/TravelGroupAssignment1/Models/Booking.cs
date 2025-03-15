using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelGroupAssignment1.Models
{
    public abstract class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public int? TripId { get; set; }

        [Display(Name = "Booking Reference Number")]
        public string? BookingReference { get; set; }

        protected Booking()
        {
            BookingReference = GenerateBookingReference();
        }

        protected virtual String GenerateBookingReference()
        {
            string date = DateTime.Now.ToString("yyMMddHHmm");
            string uniqueString = Guid.NewGuid().ToString("").Substring(0, 6);
            return date + uniqueString;
        }

        public virtual string ToEmail()
        {
            return $"Booking Reference: {BookingReference}";
        }
    }
}
