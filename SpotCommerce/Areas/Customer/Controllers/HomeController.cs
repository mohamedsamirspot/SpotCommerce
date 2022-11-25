using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotCommerce.Data;
using SpotCommerce.Models;
using SpotCommerce.Models.ViewModels;
using SpotCommerce.Utility;

namespace SpotCommerce.Controllers
{

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Item = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(), 
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()

            };


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }


            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var ItemFromDb = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                Item = ItemFromDb,
                MenuItemId = ItemFromDb.Id
            };

            return View(cartObj);
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


                if (cartFromDb==null)
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int MenuItemId)
        {
            var ItemFromDb = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == MenuItemId).FirstOrDefaultAsync();

            Wishlist Wishlist = new Wishlist()
            {
                Item = ItemFromDb,
                MenuItemId = ItemFromDb.Id
            };
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                Wishlist.ApplicationUserId = claim.Value;

                Wishlist cartFromDb = await _db.Wishlist.Where(c => c.ApplicationUserId == Wishlist.ApplicationUserId
                                                && c.MenuItemId == Wishlist.MenuItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.Wishlist.AddAsync(Wishlist);
                }

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {


                Wishlist cartObj = new Wishlist()
                {
                    Item = ItemFromDb,
                    MenuItemId = ItemFromDb.Id
                };

                return View(cartObj);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }








    }
}
