using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;
using System.Security.Claims;

namespace Kimchi_RMS.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
       
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Menu()
        {
            IEnumerable<Menu> menuList = _unitOfWork.Menu.GetAll();
            return View(menuList);
           
        }

        [HttpPost]
        [Authorize]
        public IActionResult Menu(int menuId, int count) //Method to add to card
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (count <= 0 || count >100)
            {
                TempData["error"] = "Item not added to cart as quantity is not valid.";
                return RedirectToAction("Menu");
            }

            var cartItem = _unitOfWork.ShoppingCart.GetAll()
                           .FirstOrDefault(c => c.MenuId == menuId && c.UserId == userId);

            if (cartItem == null)
            {
                // Item does not exist in cart, create a new entry
                _unitOfWork.ShoppingCart.Add(new ShoppingCart
                {
                    MenuId = menuId,
                    UserId = userId,
                    Count = count
                });
            }
            else
            {
                // Item exists, update count
                cartItem.Count += count;
                _unitOfWork.ShoppingCart.Update(cartItem);
            }

            _unitOfWork.Save();
            TempData["success"] = "Item successfully added to cart.";

            return RedirectToAction("Menu");
        }



        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult FilterByCategories(MenuCategory category)
        { 
            var filter=_unitOfWork.Menu.GetAll().Where(t => t.Category == category).ToList();
            return PartialView("_MenuVMPartial",filter);
        }
        public IActionResult SelectAll()
        {
            var filter = _unitOfWork.Menu.GetAll().ToList();
            return PartialView("_MenuVMPartial",filter);
        }

    }
}
