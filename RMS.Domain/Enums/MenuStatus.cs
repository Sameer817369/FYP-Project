using System.ComponentModel.DataAnnotations;

namespace RMS.Domain.Enums
{
    public enum MenuStatus
    {
        Available,
        [Display(Name = "Not Available")]
        Not_Available
    }
}
