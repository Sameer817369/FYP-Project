using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RMS.Application.Service.Interface;
using RMS.Domain.Enums;
using RMS.Domain.Models;
using RMS.Domain.Models.ViewModels;
using RMS.Infrastructure.Data;

namespace Kimchi_RMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRole.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
    
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            try
            {
                var users = _unitOfWork.User.GetAll().OrderByDescending(u=>u.Id).ToList();
                var userRoles = _db.UserRoles.ToList();
                var roles = _db.Roles.ToList();
                if (users == null)
                {
                    return RedirectToAction("Error", "Home", new { area = "Customer" });
                }
                foreach (var user in users) {
                    var roleId = userRoles.FirstOrDefault(u=>u.UserId==user.Id).RoleId;
                    user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                }
                return View(users);
            }
            catch(Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return RedirectToAction("Error", "Home", new { area = "Customer" });
            }
        }

        public IActionResult Update(string userId)
        {
            try 
            {
              
                var role = _db.Roles.ToList();
                var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
                RoleVM roleVM = new RoleVM()
                {
                    Users = _unitOfWork.User.GetById(u => u.Id == userId),
                    RoleList = role.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Name
                    })
                };
                roleVM.Users.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
                if (roleVM == null)
                {
                    TempData["error"] = "User not found";
                    return View();
                }
                return View(roleVM);
            }
            catch(Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }

        }
        [HttpPost]
        public IActionResult Update(RoleVM roleVM)
        {
            try
            {
                var role = _db.Roles.ToList();
                var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleVM.Users.Id).RoleId;
                var oldRole = role.FirstOrDefault(u => u.Id == roleId).Name;
                if(roleVM.Users.Role != oldRole)
                {
                    User user = _unitOfWork.User.GetById(u => u.Id == roleVM.Users.Id);
                    _unitOfWork.Save();
                    _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, roleVM.Users.Role).GetAwaiter().GetResult();
                    TempData["success"] = "User Role Updated Successfully";

                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["error"] = $"User not updated {ex.Message}";
                return View();
            }
          
        }

        [HttpPost]
        public IActionResult Delete(string userId)
        {
            try
            {
                var userToDelete = _unitOfWork.User.GetById(u => u.Id == userId);
                if (userToDelete == null)
                {
                    TempData["error"] = "User not found";
                }
                else
                {
                    _unitOfWork.User.Delete(userToDelete);
                    TempData["success"] = "User successfylly deleted";
                    _unitOfWork.Save();
                }
             
            }
            catch (Exception ex)
            {
                TempData["error"] = $"User not deleted {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult FilterByStatus(string status)
        {
            IEnumerable<User> filteredUser;

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<UserStatus>(status, out var parsedStatus))
            {
                filteredUser = _unitOfWork.User.GetAll(u => u.EmailConfirmed == (parsedStatus == UserStatus.Verified));
            }
            else
            {
                filteredUser = _unitOfWork.User.GetAll();
            }

            if (!filteredUser.Any())
            {
                return PartialView("~/Areas/Admin/Views/_NotFoundPartial.cshtml");
            }

            return PartialView("_UserTablePartial", filteredUser);
        }
    }
}
    

