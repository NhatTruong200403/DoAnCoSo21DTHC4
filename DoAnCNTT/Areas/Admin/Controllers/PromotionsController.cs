using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DoAnCNTT.Models.Utilities;

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PromotionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Promotions
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return View(promotions);
        }

        // GET: Admin/Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // GET: Admin/Promotions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,DiscountValue,ExpiredDate,Id,IsDeleted")] Promotion promotion)
        {
            var user = await _userManager.GetUserAsync(User);
            promotion.CreatedById = user!.Id;
            promotion.CreatedOn = DateTime.Now;
            promotion.ModifiedOn = null;
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Admin/Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            ViewData["CreateDate"] = promotion.CreatedOn;
            return View(promotion);
        }
        
        //Hàm trả về giá trị khyến mãi cũ đang edit
        public async Task<Promotion?> GetExistingPromotion(int id) 
            => await _context.Promotions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        // POST: Admin/Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Content,DiscountValue,ExpiredDate,Id,IsDeleted")] Promotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User); //Lấy thông tin user
            var existingPromotion = await GetExistingPromotion(id); // Lấy giá trị cũ của khuyến mãi
            promotion.CreatedOn = existingPromotion!.CreatedOn;
            promotion.CreatedById = existingPromotion.CreatedById;
            promotion.ModifiedById = existingPromotion!.ModifiedById;
            promotion.ModifiedOn = existingPromotion.ModifiedOn;
            if (promotion.ExpiredDate == null) 
            {
                promotion.ExpiredDate = existingPromotion.ExpiredDate;
            }    
            //Kiểm tra dữ liệu có thay đổi thì cập nhật ModifiedById, ModifiedOn
            bool hasChanges = EditHelper<Promotion>.HasChanges(promotion, existingPromotion); //Hàm kiểm tra
            EditHelper<Promotion>.SetModifiedIfNecessary(promotion, hasChanges, existingPromotion, user!.Id); //Hàm cập nhật nếu thay đổi
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.Id))
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
            return View(promotion);
        }



        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.Id == id);
        }
    }
}
