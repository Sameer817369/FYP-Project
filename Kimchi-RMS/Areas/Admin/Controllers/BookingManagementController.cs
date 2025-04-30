using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
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
            var bookings = _unitOfWork.Booking.GetAll().OrderByDescending(u=>u.BookingDate).ToList();
            return View(bookings);
        }

        public IActionResult BookingComplete(int bookingId)
        {
            var bookingFromDb = _unitOfWork.Booking.GetById(u => u.Id == bookingId);
            var user = _unitOfWork.User.GetById(u => u.Id == bookingFromDb.UserId);
            if (bookingFromDb == null)
            {
                return NotFound("User not found");
            }
            if (bookingFromDb.Status == BookingStatus.Pending)
            {
                bookingFromDb.Status = BookingStatus.Complete;
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

        public IActionResult Delete(int bookingId)
        {
            var bookingToRemove = _unitOfWork.Booking.GetById(u => u.Id == bookingId);
            if (bookingToRemove == null)
            {
                TempData["error"] = "Booking Id not found";
                return RedirectToAction(nameof(Index));
            }
            _unitOfWork.Booking.Delete(bookingToRemove);
            _unitOfWork.Save();
            TempData["success"] = "Booking successfully removed";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult FilterByStatus(BookingStatus? status)
        {
            var filteredBookings = status.HasValue 
                ? _unitOfWork.Booking.GetAll(u=>u.Status == status.Value)
                : _unitOfWork.Booking.GetAll();

            if (!filteredBookings.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_BookingTablePartial", filteredBookings);
        }
        public IActionResult FilterByDate(DateOnly? startDate, DateOnly? endDate)
        {
            var filteredBookings = startDate.HasValue && endDate.HasValue
                ? _unitOfWork.Booking.GetAll(u => u.ArrivalDate >= startDate.Value && u.ArrivalDate <= endDate.Value)
                : _unitOfWork.Booking.GetAll();
            if (!filteredBookings.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            else
            {
                return PartialView("_BookingTablePartial", filteredBookings);
            }

        }
    }
}
