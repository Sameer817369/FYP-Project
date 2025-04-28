using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RMS.Domain.Models
{
    public class Employee
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [Range(1, float.MaxValue, ErrorMessage ="Salary must be at least 1")]
        public float Salary { get; set; }
        [Required]
        public DateOnly HiredDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        public Roles Role {  get; set; } 
    }
}
