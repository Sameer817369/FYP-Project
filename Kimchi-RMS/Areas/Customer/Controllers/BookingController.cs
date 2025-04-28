using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;
using System.Security.Claims;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public BookingController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Error", "Home", new { area = "customer" });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Booking booking = new();
            var loggedUserInfo = booking.User;
            loggedUserInfo = _unitOfWork.User.GetById(u => u.Id == userId);
            booking.Name = loggedUserInfo.Name;
            booking.Email = loggedUserInfo.Email;
            booking.PhoneNumber = loggedUserInfo.PhoneNumber;
            booking.ArrivalDate = System.DateOnly.FromDateTime(DateTime.Now);
            booking.ArrivalTime = System.TimeOnly.FromDateTime(DateTime.Now);
            booking.Food = FoodType.Veg;
            booking.Drinks = DrinkType.SoftDrinks;
            booking.EventType = EventType.Marriage;
            return View(booking);
        }
        [HttpPost]
        [ActionName(nameof(Index))]
        public IActionResult ConfirmBooking(Booking bookingInput)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var currentUser = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.User.GetById(u => u.Id == currentUser.ToString());
            if (string.IsNullOrEmpty(currentUser))
            {
                return NotFound("User not found");
            }
            var bookedDate = _unitOfWork.Booking.GetAll(u => u.ArrivalDate == bookingInput.ArrivalDate).Any();
            if (bookedDate)
            {
                TempData["error"] = "The selected date is already booked";
                return RedirectToAction("Index");
            }
            var currentDate = System.DateOnly.FromDateTime(DateTime.Now);
            if (currentDate > bookingInput.ArrivalDate )
            {
                TempData["error"] = "Arrival Date cannot be in past";
                return RedirectToAction("Index");
            }

            Booking bookingDetail = new()
            {
                UserId = currentUser,
                Status = BookingStatus.Pending,
                Name = bookingInput.Name,
                Email = bookingInput.Email,
                PhoneNumber = bookingInput.PhoneNumber,
                BookingDate = System.DateTime.Now,
                NumberOfPlates = bookingInput.NumberOfPlates,
                Drinks = bookingInput.Drinks,
                Food = bookingInput.Food,
                EventType = bookingInput.EventType,
                ArrivalDate = bookingInput.ArrivalDate, 
                ArrivalTime = bookingInput.ArrivalTime
            };
            _unitOfWork.Booking.Add(bookingDetail);
            _unitOfWork.Save();
            try
            {
                 _emailSender.SendEmailAsync(user.Email, "Booking Made",
                     $"<p>Your Booking Has Been Made! Wait for conformation Email</p>");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to send email");
            }
            return RedirectToAction("BookingConfirmation", new { id = bookingDetail.Id });
        }
        public IActionResult BookingConfirmation(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View(id);
        }
    }
}
