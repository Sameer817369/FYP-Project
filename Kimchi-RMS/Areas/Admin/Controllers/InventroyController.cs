using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class InventroyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventroyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var inventories = _unitOfWork.Inventory.GetAll().OrderByDescending(u=>u.LastUpdated).ToList();
            return View(inventories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Inventory inventoryInput)
        {
            try
            {
                Inventory inventoryDetails = new()
                {
                    Name = inventoryInput.Name,
                    Quantity = inventoryInput.Quantity,
                    Unit = inventoryInput.Unit,
                    Category = inventoryInput.Category,
                    Status = inventoryInput.Status,
                    MinimumThreshold = 50
                };
                _unitOfWork.Inventory.Add(inventoryDetails);
                _unitOfWork.Save();
                TempData["success"] = "Inventory Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Inventory not Added{ex.Message}";
                return View(inventoryInput);
            }
        }
        public IActionResult Update(Guid inventoryId)
        {
            var inventoryFromDb = _unitOfWork.Inventory.GetById(u=> u.Id == inventoryId);
            return View(inventoryFromDb);
        }
        [HttpPost]
        public IActionResult Update(Inventory inventory)
        {
            try
            {
                _unitOfWork.Inventory.Update(inventory);
                _unitOfWork.Save();
                TempData["success"] = "Inventory Successfully Updated";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["error"] = $"Inventory not Updated {ex.Message}";
                return View(inventory);
            }
        }
        [HttpPost]
        public IActionResult Delete(Guid inventoryId)
        {
            try
            {
                var inventoryToDelete = _unitOfWork.Inventory.GetById(u => u.Id == inventoryId);
                if (inventoryToDelete == null)
                {
                    TempData["error"] = "Error! Inventory Item Not Found";
                    return RedirectToAction(nameof(Index));
                }

                _unitOfWork.Inventory.Delete(inventoryToDelete);
                _unitOfWork.Save();
                TempData["success"] = "Inventory Item Removed";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error! Inventory item not removed: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult FilterInventoryByCategory(InventoryCategory? category)
        {
            var filteredInventory = category.HasValue
                ? _unitOfWork.Inventory.GetAll(u => u.Category == category.Value)
                : _unitOfWork.Inventory.GetAll();
            if (!filteredInventory.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_InventoryTablePartial", filteredInventory);
        }
        public IActionResult FilterInventoryByStatus(InventoryStatus? status)
        {
            var filteredInventory = status.HasValue
                ? _unitOfWork.Inventory.GetAll(u => u.Status == status.Value)
                : _unitOfWork.Inventory.GetAll();
            if (!filteredInventory.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            return PartialView("_InventoryTablePartial", filteredInventory);
        }
    }
}

