using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;

namespace Kimchi_RMS.Views.Shared.ViewComponents
{
    public class FeedbackCardViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackCardViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Feedback> feedbackList = _unitOfWork.FeedBack.GetAll().OrderByDescending(f => f.CreatedAt).Take(3);
            foreach (var feedback in feedbackList)
            {
                feedback.User = _unitOfWork.User.GetById(u => u.Id == feedback.UserId);
            }
            return View(feedbackList);
        }
    }
}
