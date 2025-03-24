using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
      
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Order> orders =_unitOfWork.Order.GetAll().ToList();
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
        public IActionResult OrderComplete()
        {
            var orderFromDb = _unitOfWork.Order.GetById(u => u.Id == OrderVM.Order.Id);
            if (orderFromDb.Status == "Pending")
            {
                orderFromDb.Status = "Completed";
                _unitOfWork.Order.Update(orderFromDb);
                _unitOfWork.Save();
                TempData["Success"] = "Order Completed";
            }
            else
            {
                TempData[ "error"] = "Order is completed";
            }
            return RedirectToAction("Details", new { orderId = orderFromDb.Id });
        }
    }
}
