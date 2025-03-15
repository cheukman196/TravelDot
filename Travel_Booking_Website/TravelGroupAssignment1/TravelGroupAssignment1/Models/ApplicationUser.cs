using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TravelGroupAssignment1.Areas.CustomerManagement.Models;

namespace TravelGroupAssignment1.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int UsernameChange { get; set; } = 10; //tracks the number of times
       
        public byte[]? ProfilePic { get; set; }

        //personal information
        public DateOnly? Birthday {  get; set; }

        public string? Passport { get; set; }
        public string? Gender { get; set; }

        //preferences
        public string? HomeAirport { get; set; }
        public string? SeatPreference { get; set; }

        public string? RewardProgramName { get; set; }
        public string? RewardProgramNumber { get; set; }





    }
}

