using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotCommerce.Models.ViewModels;
using SpotCommerce.Models;
using SpotCommerce.Utility;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using SpotCommerce.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SpotCommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WishlistController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var Wishlist = await _db.Wishlist.ToListAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var wishes = _db.Wishlist.Where(c => c.ApplicationUserId == claim.Value);
            if (wishes != null)
            {
                Wishlist = wishes.ToList();
            }

            foreach (var list in Wishlist)
            {
                list.Item = await _db.Item.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);
                if (list.Item.Description != null)
                {
                    list.Item.Description = SD.ConvertToRawHtml(list.Item.Description);
                    if (list.Item.Description.Length > 100) 
                    {
                        list.Item.Description = list.Item.Description.Substring(0, 99) + "...";
                    }
                }
            }
            return View(Wishlist);

        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int MenuItemId)
        {
            var ItemFromDb = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == MenuItemId).FirstOrDefaultAsync();

            ShoppingCart ShoppingCart = new ShoppingCart()
            {
                Item = ItemFromDb,
                MenuItemId = ItemFromDb.Id
            };
            if (ModelState.IsValid)
            {

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCart.ApplicationUserId = claim.Value;


                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == ShoppingCart.ApplicationUserId
                                                && c.MenuItemId == ShoppingCart.MenuItemId).FirstOrDefaultAsync();


                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(ShoppingCart);
                }
                else
                {
                    cartFromDb.Count += ShoppingCart.Count;
                }
                await _db.SaveChangesAsync();


                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == ShoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {


                ShoppingCart cartObj = new ShoppingCart()
                {
                    Item = ItemFromDb,
                    MenuItemId = ItemFromDb.Id
                };

                return View(cartObj);
            }
        }


        public async Task<IActionResult> Remove(int wishlistId)
        {
            var wishes = await _db.Wishlist.FirstOrDefaultAsync(c => c.Id == wishlistId);

            _db.Wishlist.Remove(wishes);
            await _db.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
