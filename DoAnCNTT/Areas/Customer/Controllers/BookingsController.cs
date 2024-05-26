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

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void UpdateBookingStatus(List<Booking> bookings)
        {
            //var bookings = _context.Booking.Where(p => p.RecieveOn <= DateTime.Now && p.ReturnOn).ToList();
            foreach (var item in bookings)
            {
                if (item.RecieveOn <= DateTime.Now && item.ReturnOn >= DateTime.Now)
                {
                    item.Status = "Đang thuê";
                }
                if (item.ReturnOn < DateTime.Now)
                {
                    item.Status = "Hoàn tất";
                }
                _context.Booking.Update(item);
                _context.SaveChanges();
            }


        }

        // GET: Customer/Bookings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Booking.
                                    Include(b => b.Post).
                                    Include(b => b.Promotion).
                                    Include(b => b.User)
                                    .Where(b => b.IsDeleted == false && b.UserId == user!.Id).ToListAsync();
            ViewData["Post"] = _context.Posts.ToList();
            UpdateBookingStatus(bookings);
            return View(bookings);
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
            var post = _context.Posts.FirstOrDefault(p => p.Id == booking.PostId);
            var user = await _userManager.FindByIdAsync(post!.CreatedById);
            var originValue = booking.Total;
            if (booking.PromotionId != null)
            {
                originValue = (int)(booking.Total / (1 - booking.Promotion.DiscountValue));
            }
            ViewBag.User = user;
            ViewBag.Post = post;
            ViewBag.Sum = originValue;
            return View(booking);

        }



        [AllowAnonymous]
        [HttpPost]
        public ActionResult CalculateMiddleDate(string startDate, string endDate, decimal total)
        {

            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate);
            int numberOfDays = (int)(end - start).TotalHours;
            return Json(numberOfDays * total);
        }
        [AllowAnonymous]
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

        public async Task<bool> IsValidDate(Booking booking)
        {
            var existingBooking = await _context.Booking.
                                    OrderByDescending(b => b.Id).
                                    FirstOrDefaultAsync(b => b.PostId == booking.PostId);
            if (existingBooking == null)
            {
                return true;
            }
            if (booking.RecieveOn > existingBooking.ReturnOn || booking.ReturnOn < existingBooking.RecieveOn)
            {
                return true;
            }
            return false;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IsPay,PrePayment,Total,FinalValue,RecieveOn,ReturnOn,PostId,UserId,PromotionId,InvoiceId,Id")] Booking booking)
        {
            var user = await _userManager.GetUserAsync(User);
            booking.CreatedOn = DateTime.Now;
            booking.Status = "Đang chờ";
            booking.UserId = user!.Id;
            booking.IsRequest = false;
            var isValidDate = await IsValidDate(booking);
            if (!isValidDate)
            {
                return RedirectToAction("Details", "Posts", new { id = booking.PostId }); ;
            }
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("MomoPayment", "Invoices", new { bookingId = booking.Id });
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", booking.PostId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "Id", booking.PromotionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", booking.UserId);
            return RedirectToAction("Details", "Posts", new { id = booking.PostId }); ;
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
                booking.IsRequest = true;
                booking.Status = "Đang xử lí";
                _context.Booking.Update(booking);
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
