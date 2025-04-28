using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeController(IUnitOfWork unitOfWork)
        {   
            _unitOfWork = unitOfWork; 
        }
        public IActionResult Index()
        {
            var employeesList = _unitOfWork.Employee.GetAll().OrderByDescending(u=>u.HiredDate).ToList();
            return View(employeesList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Employee InputData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Employee employee = new()
                    {
                        Name = InputData.Name,
                        City = InputData.City,
                        Street = InputData.Street,
                        PhoneNumber = InputData.PhoneNumber,
                        Email = InputData.Email,
                        Role = InputData.Role,
                        Salary = InputData.Salary,
                    };
                    _unitOfWork.Employee.Add(employee);
                    _unitOfWork.Save();
                    TempData["success"] = "Employeed successfully added";
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {
                    TempData["errpr"] = "Employee not added";
                    return View();
                }
            }
            else
            {
                return View(InputData);
            }
        }
        public IActionResult Update(Guid employeeId)
        {
            var employeeFromDb = _unitOfWork.Employee.GetById(u => u.Id == employeeId);
            if (employeeFromDb == null)
            {
                return RedirectToAction("Error", "Home", new { area = "Customer" });
            }
            return View(employeeFromDb);
        }
        [HttpPost]
        public IActionResult Update(Employee employeeInput)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Employee.Update(employeeInput);
                    _unitOfWork.Save();
                    TempData["success"] = "Employee successfylly updated";
                    return RedirectToAction("index");
                }
                catch (Exception)
                {
                    TempData["error"] = "Employee not updated";
                    return View(employeeInput);
                }
            }
            else
            {
                TempData["error"] = "Invalid request.";
                return View(employeeInput);
            }
        }
        [HttpPost]
        public IActionResult Delete(Guid employeeId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employeeFromDb = _unitOfWork.Employee.GetById(u => u.Id == employeeId);
                    if (employeeFromDb == null)
                    {
                        TempData["error"] = "Employee not found. Cannot delete.";
                        return RedirectToAction(nameof(Index));
                    }
                    _unitOfWork.Employee.Delete(employeeFromDb);
                    _unitOfWork.Save();
                    TempData["success"] = "Employee successfully removed.";
                }
                catch (Exception)
                {
                    TempData["error"] = "Cannot delete";
                }
            }
            return RedirectToAction(nameof(Index)); 
        } 
        public IActionResult FilterByRole(Roles? roles)
        {
            var filteredEmployees = roles.HasValue
                ? _unitOfWork.Employee.GetAll(u => u.Role == roles.Value).ToList()
                : _unitOfWork.Employee.GetAll();
            if (!filteredEmployees.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            else
            {
                return PartialView("_EmployeeTablePartial", filteredEmployees);
            }   
        }
        public IActionResult FilterByDate(DateOnly ? startDate, DateOnly ? endDate)
        {
            var filteredEmployees = startDate.HasValue && endDate.HasValue
                ? _unitOfWork.Employee.GetAll(u=>u.HiredDate >=startDate.Value && u.HiredDate <= endDate.Value)
                : _unitOfWork.Employee.GetAll();
            if (!filteredEmployees.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }
            else
            {
                return PartialView("_EmployeeTablePartial", filteredEmployees);
            }

        }
    }
}

