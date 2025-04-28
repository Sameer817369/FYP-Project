using Microsoft.AspNetCore.Http;
using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class Menu
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage ="Name cannot exceed 100 characters.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be atleast 1.")]
        [Display(Name ="Price Per Plate")]
        public  double Price { get; set; }
        [Required]
        public MenuCategory Category { get; set; }
        [Required]
        public MenuStatus Status { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        [Display(Name="Image Url")]
        public string? ImgUrl { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get;set; } = DateTime.Now;
       
    }
}



