using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SpotCommerce.Data;
using SpotCommerce.Models;
using SpotCommerce.Models.ViewModels;
using SpotCommerce.Utility;

namespace SpotCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment; 

        [BindProperty] 
        public ItemViewModel ItemVM { get; set; } 
                                                          
  
        public ItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ItemVM = new ItemViewModel()
            {
                Category = _db.Category.ToList(),
                Item = new Models.Item()


            };
        }

        public async  Task<IActionResult> Index()
        {
            return View(await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View(ItemVM);
        }

        [HttpPost,ActionName("Create")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST() 
        {
            ItemVM.Item.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());


            if (!ModelState.IsValid)
            {
                return View(ItemVM);
            }

            _db.Item.Add(ItemVM.Item);
            await _db.SaveChangesAsync();

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files; 

            var ItemFromDb = await _db.Item.FindAsync(ItemVM.Item.Id); 

            if(files.Count>0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads,ItemVM.Item.Id + extension), FileMode.Create)) 
                {
                    files[0].CopyTo(filesStream);
                }
                ItemFromDb.Image = @"\images\" + ItemVM.Item.Id + extension;
            }
            else
            {
            
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultItemImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + ItemVM.Item.Id + ".png"); 
                ItemFromDb.Image = @"\images\" + ItemVM.Item.Id + ".png";
            }


            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            ItemVM.Item = await _db.Item.SingleOrDefaultAsync(m => m.Id == id);


            if(ItemVM.Item ==null)
            {
                return NotFound();
            }
            return View(ItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            ItemVM.Item.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());



            if (!ModelState.IsValid)
            {
                ItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == ItemVM.Item.CategoryId).ToListAsync();
                return View(ItemVM);
            }

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var ItemFromDb = await _db.Item.FindAsync(ItemVM.Item.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, ItemFromDb.Image.TrimStart('\\'));

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, ItemVM.Item.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                ItemFromDb.Image = @"\images\" + ItemVM.Item.Id + extension_new;
            }

            ItemFromDb.Name = ItemVM.Item.Name;
            ItemFromDb.Description = ItemVM.Item.Description;
            ItemFromDb.Price = ItemVM.Item.Price;
            ItemFromDb.CategoryId = ItemVM.Item.CategoryId;
            ItemFromDb.SubCategoryId = ItemVM.Item.SubCategoryId;



            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET : Details Item
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ItemVM.Item = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (ItemVM.Item == null)
            {
                return NotFound();
            }

            return View(ItemVM);
        }

        //GET : Delete Item
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ItemVM.Item = await _db.Item.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (ItemVM.Item == null)
            {
                return NotFound();
            }

            return View(ItemVM);
        }

        //POST Delete Item
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Item Item = await _db.Item.FindAsync(id);

            if (Item != null)
            {
                var imagePath = Path.Combine(webRootPath, Item.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.Item.Remove(Item);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }
    }
}