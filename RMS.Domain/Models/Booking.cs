using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required(ErrorMessage ="Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [Required]
        public DateOnly ArrivalDate { get; set; }
        [Required]
        public TimeOnly ArrivalTime { get; set; }
        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required]
        public FoodType Food { get; set; }
        [Required]
        public DrinkType Drinks { get; set; }
        [Required]
        [Range(1, 9999)]
        public int NumberOfPlates { get; set; }
        [Required]
        public EventType EventType { get; set; }
        public BookingStatus Status { get; set; }

    }
}
