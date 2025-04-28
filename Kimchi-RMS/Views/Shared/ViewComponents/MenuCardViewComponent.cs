using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;

namespace Kimchi_RMS.Views.Shared.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        //dependency injection
        private readonly IUnitOfWork _unitOfWork;

        public MenuViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //method to dynamically call the food menu items in the index page
        public async Task<IViewComponentResult> InvokeAsync(string title)
        {
            IEnumerable<Menu> foodItems;

            if (title == "Kimchi Specials")
            {
                foodItems = _unitOfWork.Menu.GetAll().Where(f => f.Category == MenuCategory.Kimchi_Special).OrderByDescending(f=>f.CreatedDate).Take(5);
            }
            else
            {
                foodItems = _unitOfWork.Menu.GetAll().Where(f => f.Category != MenuCategory.Kimchi_Special).OrderByDescending(f => f.CreatedDate).Take(5); ;
            }

            ViewBag.Title = title;
            return View(foodItems.ToList());  // Ensure the view receives a filtered list
        }
    }
}




