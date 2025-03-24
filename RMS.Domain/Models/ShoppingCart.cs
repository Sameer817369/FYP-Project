using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        [ValidateNever]
        public Menu Menu { get; set; }
        [Range(1,100,ErrorMessage ="Invalid! Enter number between 1 and 100")]
        public int Count { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public User User { get; set; }

        public const string SessionCart = "ShoppingCart";
    }
}
