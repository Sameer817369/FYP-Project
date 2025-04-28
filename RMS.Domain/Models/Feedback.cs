using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }
        [Required(ErrorMessage ="Give a rating to submit the feedback")]
        [Range(1,5,ErrorMessage ="Give an rating")]
        public int Rating { get; set; }
        public string ? Description { get; set; }
        public Quality ? FoodQuality { get; set; }
        public Quality ? ServiceQuality { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
