using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            double orderTotal = 0;
            ShoppingCartVM = new()
            {
                ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId),
                Order = new()
            };
            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                var menuItems = _unitOfWork.Menu.GetAll(u => u.Id == cart.MenuId).FirstOrDefault();
                if (menuItems != null)
                {
                    double individualTotal = cart.Menu.Price * cart.Count;
                    orderTotal += individualTotal;  
                }
            }
            ShoppingCartVM.Order.TotalAmount = orderTotal;
            return View(ShoppingCartVM);
        }
        public IActionResult OrderSummary()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            double orderTotal = 0;
            ShoppingCartVM = new()
            {
                ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId),
                Order = new()
            };
            var loggedUserInfo = ShoppingCartVM.Order.User;
            loggedUserInfo = _unitOfWork.User.GetById(u => u.Id == userId);
            var orderInfo = ShoppingCartVM.Order;
            orderInfo.Name = loggedUserInfo.Name;
            orderInfo.Address = loggedUserInfo.Address;
            orderInfo.City = loggedUserInfo.City;
            orderInfo.PhoneNumber = loggedUserInfo.PhoneNumber;
      
            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                var menuList = _unitOfWork.Menu.GetAll(u => u.Id == cart.MenuId);
                if (menuList != null)
                {
                    double individualTotal = cart.Menu.Price * cart.Count;
                    orderTotal += individualTotal; 
                }
                ShoppingCartVM.Order.TotalAmount = orderTotal;
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("OrderSummary")]
        public IActionResult OrderSummaryPost(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            double orderTotal = 0;

            ShoppingCartVM = new()
            {
                ShoppingCardList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId),
                Order = shoppingCartVM.Order
            };

            ShoppingCartVM.Order.OrderDate = System.DateTime.Now;
            ShoppingCartVM.Order.UserId = userId;
            ShoppingCartVM.Order.Status = Status.Pending;

            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                var menuItem = _unitOfWork.Menu.GetAll(u => u.Id == cart.MenuId).FirstOrDefault();
                if (menuItem != null)
                {
                    double individualTotal = cart.Menu.Price * cart.Count;
                    orderTotal += individualTotal;
                }
            }
            ShoppingCartVM.Order.TotalAmount = orderTotal;
            // Serializing the Order object to a string
            string serializedOrder = JsonConvert.SerializeObject(ShoppingCartVM.Order);
            // Store order temporarily in session
            HttpContext.Session.SetString("PendingOrder", serializedOrder);
            // Redirect to eSewa Payment
            return RedirectToAction("EsewaPayment", "Payment");
        }

        public async Task<IActionResult> OrderConformation(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.User.GetById(u => u.Id == userId);
            var order = _unitOfWork.Order.GetById(u => u.UserId == user.Id);
            // Fetch the cart based on UserId
            var cartToRemove = _unitOfWork.ShoppingCart.GetById(u => u.UserId == userId);

            // Ensure cart exists before deleting
            if (cartToRemove != null)
            {
                _unitOfWork.ShoppingCart.Delete(cartToRemove);
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
            if(user == null)
            {
                return NotFound("User not found");
            }
            try
            {
                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",

                      $"<h2>Order Confirmation</h2>" +
                      $"<p>Hi <strong>{order.Name}</strong>,</p>" +
                      $"<p>Your order has been successfully placed!</p>" +
                      $"<p>Thank you for ordering from us.</p>" +
                      $"<hr>" +
                      $"<h4>Receipt</h4>" +
                      $"<p><strong>Id:</strong> {order.Id}</p>" +
                      $"<p><strong>Items:</strong> {order.Name}</p>" +
                      $"<p><strong>Order Date:</strong>{order.OrderDate}</p>" +
                      $"<p><strong>Total:</strong> <span class='text-success fw-bold'>{order.TotalAmount}</span></p>" +
                      $"<p>We hope to serve you again soon!</p>");
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Failed to send emial");
            }
            return View(id);
        }


        public IActionResult Add(int cartId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.GetById(u => u.Id == cartId);
            if (cardFromDb == null)
            {
                TempData["Error"] = "Cart item not found.";
                return RedirectToAction("Index");
            }
            cardFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cardFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Cart item increased.";
            return RedirectToAction("Index");
        }
        public IActionResult Minus(int cartId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.GetById(u => u.Id == cartId);
            if (cardFromDb == null)
            {
                TempData["Error"] = "Cart item not found.";
                return RedirectToAction("Index");
            }
            else
            {
                if (cardFromDb.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Delete(cardFromDb);
                    HttpContext.Session.SetInt32(ShoppingCart.SessionCart,
                         _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cardFromDb.UserId).Count() - 1);
                }
                else
                {
                    cardFromDb.Count -= 1;
                    _unitOfWork.ShoppingCart.Update(cardFromDb);
                }

                _unitOfWork.Save();
                TempData["Error"] = "Cart item decreased.";
                return RedirectToAction("Index");
            }
           
        }
        public IActionResult Remove(int cartId)
        {
            var cardFromDb = _unitOfWork.ShoppingCart.GetById(u => u.Id == cartId);
            if (cardFromDb == null)
            {
                TempData["Error"] = "Cart item not found.";
                return RedirectToAction("Index");
            }
            _unitOfWork.ShoppingCart.Delete(cardFromDb);
            HttpContext.Session.SetInt32(ShoppingCart.SessionCart,
                 _unitOfWork.ShoppingCart.GetAll(u => u.UserId == cardFromDb.UserId).Count()-1);
            _unitOfWork.Save();
            TempData["success"] = "Cart item removed.";
            return RedirectToAction("Index");
        }
    }
}
