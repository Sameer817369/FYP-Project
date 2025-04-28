using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Domain.Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Order { get; set; }

        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
