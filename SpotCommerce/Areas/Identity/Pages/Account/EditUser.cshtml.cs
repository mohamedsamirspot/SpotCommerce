using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SpotCommerce.Data;
using SpotCommerce.Models;
using SpotCommerce.Utility;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpotCommerce.Areas.Identity.Pages.Account
{
    public class EditUserModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ApplicationDbContext _db;
        public EditUserModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }


        public string ReturnUrl { get; set; }
        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync( string returnUrl = null)
        {
            string role = Request.Form["rdUserRole"].ToString();
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id != id);
        
                    if (role == SD.OperatorUser)
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
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }


                    // hna 3shan b3d ma acreate new user ywdene 3la sf7t el users brdo
                    return RedirectToAction("Index", "User", new { area = "Admin" });

            }
        



    }
}
