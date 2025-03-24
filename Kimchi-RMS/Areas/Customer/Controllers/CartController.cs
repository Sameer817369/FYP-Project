using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;
using System.Security.Claims;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
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
            ShoppingCartVM.Order.Status = "Pending";

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
            _unitOfWork.Order.Add(ShoppingCartVM.Order);
            _unitOfWork.Save();
            foreach(var cart in ShoppingCartVM.ShoppingCardList)
            {
                OrderDetail orderDetail = new()
                {
                    MenuId = cart.MenuId,
                    OrderId = ShoppingCartVM.Order.Id,
                    Price = cart.Menu.Price,
                    Quantity = cart.Count    
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            return RedirectToAction("OrderConformation", new { id = ShoppingCartVM.Order.Id });
        }
        public IActionResult OrderConformation(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Fetch the cart based on UserId
            var cartToRemove = _unitOfWork.ShoppingCart.GetById(u => u.UserId == userId);

            // Ensure cart exists before deleting
            if (cartToRemove != null)
            {
                _unitOfWork.ShoppingCart.Delete(cartToRemove);
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
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
