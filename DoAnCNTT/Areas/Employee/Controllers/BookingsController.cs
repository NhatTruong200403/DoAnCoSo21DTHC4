using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DoAnCNTT.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Admin,Employee")]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee/Bookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Booking
                                            .Include(b => b.Post)
                                            .Include(b => b.Promotion)
                                            .Include(b => b.User)
                                            .Where(b => b.IsRequest == true);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Bookings/Details/5
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
            
            var originValue = booking.Total;
            if (booking.PromotionId != null)
            {
                originValue = (int)(booking.Total / (1 - booking.Promotion.DiscountValue));
            }
            ViewBag.Post = post;
            ViewBag.Sum = originValue;
            return View(booking);
        }

        //Hoàn tiền dựa theo đơn đặt cọc
        public async Task Refund(Booking booking)
        {
            var invoice = await _context.Invoices.
                                    Where(i => i.BookingId == booking.Id).
                                    OrderByDescending(b => b.Id).
                                    FirstOrDefaultAsync();
            var refundValue = (double)invoice!.Total;
            var refundHours = (booking.RecieveOn - DateTime.Now).TotalHours;
            var createHours = (booking.CreatedOn - DateTime.Now).TotalHours;
            if (refundHours > 168 && createHours > 1)
            {
                refundValue = refundValue * 0.7;
            }
            else if(refundHours <= 168 && createHours > 1) 
            {
                refundValue = 0;
            }
            else if(createHours == 1)
            {
                refundValue = (double)invoice.Total;
            }    
            if (invoice != null)
            {
  
                var refundInvoice = new Invoice()
                {
                    Total = -(decimal)refundValue,
                    ReturnOn = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    BookingId = booking.Id,
                };
                _context.Invoices.Add(refundInvoice);
                await _context.SaveChangesAsync();
            }
        }

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

        //Xác nhận yêu cầu hủy đặt cọc của khách hàng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                booking.IsDeleted = true;
                booking.Status = "Đã trả cọc"; 
                _context.Booking.Update(booking);
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == booking!.PostId);
                post!.RideNumber--;
                _context.Posts.Update(post);
            }
            await _context.SaveChangesAsync();
            await Refund(booking!);
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
