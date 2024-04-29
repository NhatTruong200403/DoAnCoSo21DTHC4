using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity;
using DoAnCNTT.Models.Utilities;

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AmenitiesController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Amenities.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (amenity == null)
            {
                return NotFound();
            }

            return View(amenity);
        }
        public async Task<string> SaveImage(IFormFile file)
        {
            var savePath = "./wwwroot/images/";
            var filePath = Path.Combine(savePath + file?.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file!.CopyToAsync(fileStream);
            }
            return "/images/" + file.FileName;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IsDeleted")] Amenity amenity,IFormFile? IconImage)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                amenity.CreatedById = user.Id;
                amenity.CreatedOn = DateTime.Now;
                if(IconImage != null)
                {
                    amenity.IconImage = await SaveImage(IconImage);
                }    
                _context.Add(amenity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return View(amenity);
        }
        public async Task<Amenity?> GetExistingAmenity(int id)
            => await _context.Amenities.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsDeleted")] Amenity amenity,IFormFile? IconImage)
        {
            if (id != amenity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var existingAmenity = await GetExistingAmenity(id);
                    if (IconImage != null)
                    {
                        amenity.IconImage = await SaveImage(IconImage);
                    }
                    else
                    {
                        amenity.IconImage = existingAmenity!.IconImage;
                    }
                    amenity.CreatedOn = existingAmenity.CreatedOn;
                    amenity.CreatedById = existingAmenity.CreatedById;
                    amenity.ModifiedOn = existingAmenity.ModifiedOn;
                    amenity.ModifiedById = existingAmenity.ModifiedById;
                    bool hasChanges = EditHelper<Amenity>.HasChanges(amenity, existingAmenity); //Hàm kiểm tra
                    EditHelper<Amenity>.SetModifiedIfNecessary(amenity, hasChanges, existingAmenity, user!.Id); //Hàm cập nhật nếu thay đổi
                    _context.Update(amenity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmenityExists(amenity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(amenity);
        }
        private bool AmenityExists(int id)
        {
            return _context.Amenities.Any(e => e.Id == id);
        }
    }
}
