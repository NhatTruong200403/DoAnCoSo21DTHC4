// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DoAnCNTT.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "FullName")]
            public string FullName { get; set; }
            [Display(Name = "Images")]
            public string Images { get; set; }
            [Display(Name = "Birthday")]
            public DateTime? Birthday { get; set; }
        }
        private async Task<string> SaveImage(IFormFile image)
        {

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "ImageUser");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var savePath = Path.Combine(uploadPath, image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/ImageUser/" + image.FileName;
        }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            DateTime? dateTime = null;
            
            if (user.Birthday != null)
            {
                dateTime = (DateTime)user.Birthday;
            }
            
            var name = user.Name;
            Username = userName;

            Input = new InputModel
            {
                FullName = name,
                PhoneNumber = phoneNumber,
                Birthday = dateTime
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

            var currentUser = await _context.Users.FindAsync(user.Id);
            if (currentUser == null)
            {
                return NotFound($"Unable to load user with ID '{user.Id}'.");
            }
            if (Input.FullName != null)
            {
                currentUser.Name = Input.FullName;
            }
            else
            {
                currentUser.Name = currentUser.Name;
            }
            if (Request.Form.Files.Count != 0)
            {
                foreach (var file in Request.Form.Files)
                {
                    var newImagePath = await SaveImage(file);

                    if (file.Name == "Input.Images") 
                    {
                        currentUser.Image = newImagePath;
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                currentUser.Image = user.Image;
                await _context.SaveChangesAsync();
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (Input.Birthday != null)
            {
                
                DateTime birthdayDateTime = Input.Birthday.Value;               
                currentUser.Birthday = birthdayDateTime;
                await _context.SaveChangesAsync();
            }
            else
            {
                currentUser.Birthday = currentUser.Birthday;
                await _context.SaveChangesAsync();
            }
            
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
