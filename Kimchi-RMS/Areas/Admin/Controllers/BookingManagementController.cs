using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class BookingManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public BookingManagementController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            List<Booking> bookings = _unitOfWork.Booking.GetAll().ToList();
            return View(bookings);
        }

        public IActionResult BookingComplete(int bookingId)
        {
            var bookingFromDb = _unitOfWork.Booking.GetById(u => u.Id == bookingId);
            var user = _unitOfWork.User.GetById(u =>u.Id == bookingFromDb.UserId);
            if(bookingFromDb == null)
            {
                return NotFound("User not found");
            }
            if (bookingFromDb.Status == "Pending")
            {
                bookingFromDb.Status = "Approved";
                _unitOfWork.Booking.update(bookingFromDb);
                _unitOfWork.Save();
                TempData["Success"] = "Booking Completed";
                try
                {
                    _emailSender.SendEmailAsync(user.Email, "Booking Conformed",
                        $"<p>Your Booking has been conformed for {bookingFromDb.ArrivalDate} at {bookingFromDb.ArrivalTime}</p>");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Failed to send email");
                }
            }
            else
            {
                TempData["error"] = "Booking is completed";
            }
         
            return RedirectToAction("Index", new { BookingId = bookingFromDb.Id });
        }
    }
}
