// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kimchi_RMS.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Phone number")]
            [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits.")]
            [Required]
            public string PhoneNumber { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Street { get; set; }
            [Required]
            public string City { get; set; }

        }

        private async Task LoadAsync(IdentityUser user)
        {

            var userInfo = _unitOfWork.User.GetById(u => u.Id == user.Id);
            Input = new InputModel
            {
                PhoneNumber = userInfo.PhoneNumber,
                Name = userInfo.Name,
                Street = userInfo.Address,
                City = userInfo.City 
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            // Update Additional Properties (Name, Address, City)
            var userInfo = _unitOfWork.User.GetById(u => u.Id == user.Id);
            if (userInfo.PhoneNumber != Input.PhoneNumber ||
                userInfo.Name != Input.Name ||
                userInfo.Address != Input.Street ||
                userInfo.City != Input.City)
            {
                userInfo.PhoneNumber = Input.PhoneNumber;
                userInfo.Name = Input.Name;
                userInfo.Address = Input.Street;
                userInfo.City = Input.City;
                _unitOfWork.User.Update(userInfo);
                _unitOfWork.Save();
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
            }
            else
            {
                StatusMessage = "Error: Nothing changed to update";
            }
            return RedirectToPage();
        }
    }
}
