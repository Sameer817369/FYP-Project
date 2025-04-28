using Microsoft.AspNetCore.Mvc.Rendering;

namespace RMS.Domain.Models.ViewModels
{
    public class RoleVM
    {
        public User Users { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
