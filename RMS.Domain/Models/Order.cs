using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]
        public Status Status { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Address cannot exceed 100 characters.")]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
