using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;
using System.Security.Claims;
using System.Web.WebPages;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
      
        public OrderController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var orders =_unitOfWork.Order.GetAll().OrderByDescending(u=>u.OrderDate).ToList();
            foreach(var order in orders)
            {
                order.User = _unitOfWork.User.GetById(u=>u.Id == order.UserId);
            }
            return View(orders);
        }
        public IActionResult Details(int ? orderId)
        {
            var allordersDetails = _unitOfWork.OrderDetail.GetAll();

            OrderVM = new()
            {
                Order = _unitOfWork.Order.GetById(u => u.Id == orderId),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId)
            };
            foreach (var items in OrderVM.OrderDetail)
            {
                var menulist = _unitOfWork.Menu.GetById(m => m.Id == items.MenuId);
            }
            return View(OrderVM);
        }
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _unitOfWork.Order.GetById(u => u.Id == OrderVM.Order.Id);
            var updatedOrder = OrderVM.Order;
            orderFromDb.Name = updatedOrder.Name;
            orderFromDb.PhoneNumber = updatedOrder.PhoneNumber;
            orderFromDb.Address = updatedOrder.Address;
            orderFromDb.OrderDate = updatedOrder.OrderDate;
            orderFromDb.City = updatedOrder.City;
            try
            {
                _unitOfWork.Order.Update(orderFromDb);
                _unitOfWork.Save();
                TempData["Success"] = "Order Details Update Successfully";
                return RedirectToAction("Details", new { orderId = orderFromDb.Id });
            }
            catch (Exception)
            {
                TempData["error"] = "Error! Order Details Not Updated";
                return RedirectToAction("Details", new {orderId = orderFromDb.Id});
            }
        }
        [HttpPost]
        public async Task<IActionResult> OrderComplete()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var orderFromDb = _unitOfWork.Order.GetById(u => u.Id == OrderVM.Order.Id);
            var user = _unitOfWork.User.GetById(u => u.Id == orderFromDb.UserId);

            if (orderFromDb.Status == Status.Pending )
            {
                orderFromDb.Status = Status.Complete;
                _unitOfWork.Order.Update(orderFromDb);
                _unitOfWork.Save();
                TempData["Success"] = "Order Completed";
                try
                {
                    await _emailSender.SendEmailAsync(user.Email, "Order Delivery",
                         $"<p>Your Order place on {orderFromDb.OrderDate} Has been given to delivery!</p>");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Failed to send email");
                }
            }
            else
            {
                TempData["error"] = "Order is completed";
            }
       
            return RedirectToAction("Details", new { orderId = orderFromDb.Id });
        }
        public IActionResult FilterOrderByStatus(Status? status)
        {
            var filteredOrders = status.HasValue
                ? _unitOfWork.Order.GetAll(u => u.Status == status.Value)
                : _unitOfWork.Order.GetAll();

            foreach (var order in filteredOrders)
            {
                order.User = _unitOfWork.User.GetById(u => u.Id == order.UserId);
            }

            if (!filteredOrders.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_OrderTablePartial", filteredOrders);
        }
        public IActionResult FilterByDateRange(DateTime? startDate, DateTime? endDate)
        {
            var filteredOrders = startDate.HasValue && endDate.HasValue 
                ? _unitOfWork.Order.GetAll(u=>u.OrderDate >= startDate.Value && u.OrderDate <= endDate.Value)
                : _unitOfWork.Order.GetAll();
            foreach(var order in filteredOrders)
            {
                order.User = _unitOfWork.User.GetById(u => u.Id == order.UserId);
            }
            if (!filteredOrders.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_OrderTablePartial", filteredOrders);
        }
    }
}







