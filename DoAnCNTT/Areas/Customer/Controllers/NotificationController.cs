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

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer/Notification
        public async Task<IActionResult> Index(string userId)
        {
            var notifications = _context.Posts.Where(p => p.IsDisabled == true && p.IsHidden == false && p.UserId == userId);
            return View(await notifications.ToListAsync());
        }

        // GET: Customer/Notification/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.CarType)
        //        .Include(p => p.Company)
        //        .Include(p => p.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}

        //// GET: Customer/Notification/Create
        //public IActionResult Create()
        //{
        //    ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id");
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id");
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
        //    return View();
        //}

        //// POST: Customer/Notification/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Name,Image,Description,Seat,RentLocation,HasDriver,Price,Gear,Fuel,FuelConsumed,RideNumber,AvgRating,IsAvailable,IsDisabled,CarTypeId,CompanyId,UserId,Id,CreatedById,CreatedOn,ModifiedById,ModifiedOn,IsDeleted")] Post post)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(post);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", post.CarTypeId);
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", post.CompanyId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        //    return View(post);
        //}

        //// GET: Customer/Notification/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts.FindAsync(id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", post.CarTypeId);
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", post.CompanyId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        //    return View(post);
        //}

        //// POST: Customer/Notification/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Name,Image,Description,Seat,RentLocation,HasDriver,Price,Gear,Fuel,FuelConsumed,RideNumber,AvgRating,IsAvailable,IsDisabled,CarTypeId,CompanyId,UserId,Id,CreatedById,CreatedOn,ModifiedById,ModifiedOn,IsDeleted")] Post post)
        //{
        //    if (id != post.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(post);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PostExists(post.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", post.CarTypeId);
        //    ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Id", post.CompanyId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        //    return View(post);
        //}

        // POST: Customer/Notification/Delete/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsHidden = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
