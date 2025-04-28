using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using System.Security.Claims;

namespace Kimchi_RMS.Views.Shared.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                if (HttpContext.Session.GetInt32(ShoppingCart.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(ShoppingCart.SessionCart,
                        _unitOfWork.ShoppingCart.GetAll(u => u.UserId == userId).Count());
                }
                return View(HttpContext.Session.GetInt32(ShoppingCart.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
