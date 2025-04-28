using RMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RMS.Domain.Models
{
    public class Inventory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public InventoryUnit Unit { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Qunatity cannot be less than 0")]
        public double Quantity { get; set; }
        [Required]
        [Range(1, 50, ErrorMessage = "Threshold cannot exceed 50")]
        public double MinimumThreshold { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public InventoryCategory Category { get; set; }
        public InventoryStatus Status { get; set; }
    }
}
