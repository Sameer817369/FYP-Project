using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class FeedbackManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var feedbacks = _unitOfWork.FeedBack.GetAll().OrderByDescending(u => u.CreatedAt).ToList();
            foreach(var feedback in feedbacks)
            {
                feedback.User = _unitOfWork.User.GetById(u => u.Id == feedback.UserId);
            }
            return View(feedbacks);
        }
        [HttpPost]
        public IActionResult Delete(Guid feedbackId) 
        {
            try
            {
                var feedbacksToDelete = _unitOfWork.FeedBack.GetById(u => u.Id == feedbackId);
                if (feedbacksToDelete == null)
                {
                    TempData["error"] = "Error! feedback not found";
                    return RedirectToAction("Index");
                }
                _unitOfWork.FeedBack.Delete(feedbacksToDelete);
                _unitOfWork.Save();
                TempData["success"] = "Feedback Deleted Successfully";
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                TempData["error"] = "Error! Feedback Not Deleted";
                return RedirectToAction("Index"); 
            }
        }
        public IActionResult FilterByDate(DateTime ? startDate, DateTime ? endDate)
        {
            var filteredFeedbacks = startDate.HasValue && endDate.HasValue
                ? _unitOfWork.FeedBack.GetAll(u => u.CreatedAt >= startDate.Value && u.CreatedAt <= endDate.Value)
                : _unitOfWork.FeedBack.GetAll();
            foreach(var feedback in filteredFeedbacks)
            {
                feedback.User = _unitOfWork.User.GetById(u => u.Id == feedback.UserId);
            }
            if (!filteredFeedbacks.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_FeedbackTablePartial", filteredFeedbacks);
        }
    }
}
