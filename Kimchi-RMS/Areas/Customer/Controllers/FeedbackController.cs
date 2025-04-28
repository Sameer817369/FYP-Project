using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using System.Security.Claims;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(UserRole.Role_Admin))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }
        [HttpPost]
        [ActionName(nameof(Index))]
        public IActionResult FeedbackPost(Feedback feedbackInput)
        {
            if(feedbackInput.Rating == 0)
            {
                TempData["error"] = "Selecting rating is must";
                return RedirectToAction("Index");

            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUser = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrEmpty(currentUser))
            {
                return NotFound("User not found");
            }
            Feedback feedbackDetail = new()
            {
                UserId = currentUser,
                CreatedAt = DateTime.UtcNow,
                Description = feedbackInput.Description,
                Rating = feedbackInput.Rating,
                ServiceQuality = feedbackInput.ServiceQuality,
                FoodQuality = feedbackInput.FoodQuality,
            };
            _unitOfWork.FeedBack.Add(feedbackDetail);
            _unitOfWork.Save();
            return RedirectToAction("FeedbackConformation");
        }
        public IActionResult FeedbackConformation()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }
    }
}
