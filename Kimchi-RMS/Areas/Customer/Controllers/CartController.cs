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
                Order = new(),
            };
            foreach (var cart in ShoppingCartVM.ShoppingCardList)
            {
                var menuList = _unitOfWork.Menu.GetAll(u => u.Id == cart.Id);
                if (menuList != null)
                {
                    double individualTotal = cart.Menu.Price * cart.Count;
                    cart.Menu.MenuTotal = individualTotal;
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
                Order = new(),
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
                var menuList = _unitOfWork.Menu.GetAll(u => u.Id == cart.Id);
                if (menuList != null)
                {
                    double individualTotal = cart.Menu.Price * cart.Count;
                    cart.Menu.Price = individualTotal;
                    orderTotal += individualTotal; 
                }
                ShoppingCartVM.Order.TotalAmount = orderTotal;
            }
            return View(ShoppingCartVM);
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

                }
                else
                {
                    cardFromDb.Count -= 1;
                    _unitOfWork.ShoppingCart.Update(cardFromDb);
                }

                _unitOfWork.Save();
                TempData["success"] = "Cart item decreased.";
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
            _unitOfWork.Save();
            TempData["success"] = "Cart item removed.";
            return RedirectToAction("Index");
        }
    }
}
