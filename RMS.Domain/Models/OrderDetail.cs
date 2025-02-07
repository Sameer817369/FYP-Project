using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Order { get; set; }
        [Required]
        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        [ValidateNever]
        public Menu Menu { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Price { get; set; }//price at the time of purchase
    }
}
