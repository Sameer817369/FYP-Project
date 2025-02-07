using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =UserRole.Role_Admin)]
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MenuController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Menu> menuList = _unitOfWork.Menu.GetAll().ToList();
            return View(menuList); 
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Menu items)
        {
            if (ModelState.IsValid)
            {
                if(items.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(items.Image.FileName).ToLower();
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images/menuImages/");
                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    items.Image.CopyTo(fileStream);
                    items.ImgUrl = @"images/menuImages/" + fileName;
                }
                else
                {
                    items.ImgUrl = "https://placehold.co/600x400";
                }
                try
                {
                    _unitOfWork.Menu.Add(items);
                    _unitOfWork.Save();
                    TempData["success"] = "Menu Item Created Successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["error"] = "Menu Item Not Created";
                    return View(items);
                }
            }
            else
            {
                return View(items);
            }
        }
        public IActionResult Update(int? menuId)
        {
            if (menuId == null || menuId == 0)
            {
                return RedirectToAction("Error", "Home", new { area = "Customer" });
            }
            Menu? menuFromDb = _unitOfWork.Menu.GetById(u => u.Id == menuId);
            if (menuFromDb == null)
            {
                return RedirectToAction("Error", "Home", new { area = "Customer" });
            }
            return View(menuFromDb);
        }
        [HttpPost]
        public IActionResult Update(Menu items)
        {
            if (ModelState.IsValid)
            {
                if (items.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(items.Image.FileName).ToLower();
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images/menuImages/");
                    if (!string.IsNullOrEmpty(items.ImgUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, items.ImgUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    items.Image.CopyTo(fileStream); 
                    items.ImgUrl = @"images/menuImages/" + fileName;
                }
           
                try
                {
                    _unitOfWork.Menu.Update(items);
                    _unitOfWork.Save();
                    TempData["success"] = "Menu Item Updated Successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["error"] = "Error! Menu Item Not Updated";
                    return NotFound();
                }
            }
            else
            {
                return View(items);
            }
        }
        public IActionResult Delete(int? menuId)
        {
            if (menuId == null || menuId == 0)
            {
                return RedirectToAction("Error", "Home", new {area="Customer"});
            }
            Menu? menuFromDb = _unitOfWork.Menu.GetById(u => u.Id == menuId);
            if (menuFromDb == null)
            {
              return RedirectToAction("Error", "Home", new { area = "Customer" });
            }
            return View(menuFromDb);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Delete(int menuId)
        {
            try
            {
                var item = _unitOfWork.Menu.GetById(u => u.Id == menuId);
                if (item == null)
                {
                    TempData["error"] = "Error! Menu Item Not Found";
                    return RedirectToAction("Index");
                }

                // Delete image if it exists
                if (!string.IsNullOrEmpty(item.ImgUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.ImgUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Menu.Delete(item);
                _unitOfWork.Save();

                TempData["success"] = "Menu Item Deleted Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error deleting menu item: {ex.Message}");

                TempData["error"] = "Error! Menu Item Not Deleted";
                return RedirectToAction("Index"); // Redirect instead of returning NotFound()
            }
        }

    }
}
