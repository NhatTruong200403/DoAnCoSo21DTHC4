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

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Booking.Include(b => b.Post).Include(b => b.Promotion).Include(b => b.User).Where(b => b.IsDeleted == false);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Post)
                .Include(b => b.Promotion)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        //// GET: Customer/Bookings/Create
        //public async Task<IActionResult> Create(string userId, int postId)
        //{
        //    var post = await _context.Posts.FindAsync(postId);
        //    var booking = new Booking()
        //    {
        //        PostId = postId,
        //        UserId = userId,
        //        CreatedOn = DateTime.Now,
        //        RecieveOn = DateTime.Today,
        //        ReturnOn = DateTime.Today,
        //        Total = post!.Price,
        //        PrePayment = post!.Price * (decimal)0.3,
        //        FinalValue = post!.Price
        //    };
        //    var promotions = _context.Promotions.Where(p => p.IsDeleted == false).ToList();
        //    ViewData["Promotions"] = new SelectList(promotions, "Id", "Content");
        //    //return View(booking);
        //    return View(booking);
        //}


 /*       public async Task<IActionResult> Create(string userId, int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            var booking = new Booking()
            {
                PostId = postId,
                UserId = userId,
                CreatedOn = DateTime.Now,
                RecieveOn = DateTime.Today,
                ReturnOn = DateTime.Today,
                Total = post!.Price,
                FinalValue = post!.Price,
                PrePayment = post!.Price * (decimal)0.3,
            };
            var promotions = _context.Promotions.Where(p => p.IsDeleted == false).ToList();
            ViewData["Promotions"] = new SelectList(promotions, "Id", "Content");
            return PartialView("Create", booking);
        }*/



        [HttpPost]
        public ActionResult CalculateMiddleDate(string startDate, string endDate, decimal total)
        {

            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate);
            int numberOfDays = (int)(end - start).TotalHours;
            return Json(numberOfDays*total);
        }

        public IActionResult CalculateFinalValue(decimal total, int promotionId)
        { 
            // Lấy giá trị Promotion tương ứng từ CSDL
            var promotion = _context.Promotions.FirstOrDefault(p => p.Id == promotionId);
            if (promotion == null)
            {
                return Json(new { total = total });
            }
            else
            {
                return Json(new { total = total - (total * promotion.DiscountValue) });
            }
        }

        // POST: Customer/Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsPay,PrePayment,Total,FinalValue,RecieveOn,ReturnOn,PostId,UserId,PromotionId,InvoiceId,Id,CreatedById,CreatedOn,ModifiedById,ModifiedOn,IsDeleted")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("MomoPayment", "Invoices", new { bookingId = booking.Id });
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", booking.PostId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id", booking.PromotionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return View(booking);
        }

        // GET: Customer/Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Post)
                .Include(b => b.Promotion)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Customer/Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
