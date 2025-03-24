using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
