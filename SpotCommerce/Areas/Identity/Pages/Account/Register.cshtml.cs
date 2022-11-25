﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SpotCommerce.Models;
using SpotCommerce.Utility;

namespace SpotCommerce.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [MaxLength(50)]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [MaxLength(50)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [MaxLength(50)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MaxLength(50)]
            public string Name { get; set; }

            [Display(Name = "Street Address")]
            [MaxLength(200)]
            public string StreetAddress { get; set; }
            [Display(Name = "Phone Number")]
            [MaxLength(50)]
            public string PhoneNumber { get; set; }
            [MaxLength(50)]
            public  string City { get; set; }
            [MaxLength(50)]
            public string State { get; set; }
            [MaxLength(10)]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();


            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    City=Input.City,
                    StreetAddress=Input.StreetAddress,
                    State=Input.State,
                    PostalCode=Input.PostalCode,
                    PhoneNumber=Input.PhoneNumber
                };
      
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                   
                    if(role==SD.OperatorUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.OperatorUser);
                    }
                    else
                    {
                        if (role == SD.FrontDeskUser)
                        {
                            await _userManager.AddToRoleAsync(user, SD.FrontDeskUser);
                        }
                        else
                        {
                            if (role == SD.ManagerUser)
                            {
                                await _userManager.AddToRoleAsync(user, SD.ManagerUser);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }


                    // hna 3shan b3d ma acreate new user ywdene 3la sf7t el users brdo
                    return RedirectToAction("Index", "User", new { area = "Admin" });

                    _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    
                  
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
