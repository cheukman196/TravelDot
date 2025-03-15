using System.ComponentModel.DataAnnotations;

namespace TravelGroupAssignment1.Models
{
    public abstract class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [Display(Name = "Author")]
        public string? Author { get; set; }

        [Required]
        [Display(Name = "Comment")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string? Content { get; set; }

        [Required]
        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "Ratings can only be between 0 and 5.")]
        public double Rating { get; set; }

        [Display(Name = "Posted Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DatePosted { get; set; }


    }
}
