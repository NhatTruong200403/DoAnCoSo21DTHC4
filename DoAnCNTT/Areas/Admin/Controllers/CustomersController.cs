using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CustomersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: CustomerController
        public async Task<ActionResult> Index()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            return View(customers);
        }


        // GET: CustomerController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var listpost = await _context.Posts
                                         .Where(p => p.CreatedById == id)
                                         .ToListAsync();
            ViewBag.listpost = listpost;

            return View(user);
        }

        public async Task<IActionResult> GetBookingHistory(string userId)
        {
            var bookings = await _context.Booking.
                                    Include(b => b.Post).
                                    Include(b => b.Promotion).
                                    Include(b => b.User)
                                    .Where(b => b.IsDeleted == false && b.UserId == userId).ToListAsync();
            ViewData["Post"] = _context.Posts.ToList();
            return View("~/Areas/Customer/Views/Bookings/Index.cshtml", bookings);
        }

        public async Task<IActionResult> GetPaymentHistory(string userId)
        {
            var invoices = await _context.Invoices
                                .Include(i => i.Booking)
                                .Where(i => i.Booking.UserId == userId).ToListAsync();
            return View("~/Areas/Customer/Views/Invoices/Index.cshtml", invoices);
        }

        // GET: CustomerController/Create
        public async Task<IActionResult> LockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                if (user.LockoutEnd < DateTime.Now || user.LockoutEnd == null)
                {
                    user.LockoutEnd = DateTime.Now.AddDays(7);
                    user.LockoutEnabled = true;
                }
                var result = await _userManager.UpdateAsync(user!);
            }
            return RedirectToAction("Index", "Customers", new { area = "Admin"});
        }

        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (user.LockoutEnd != null)
                {
                    user.LockoutEnd = DateTime.Now;
                }
                var result = await _userManager.UpdateAsync(user!);
            }
            return RedirectToAction("Index", "Customers", new { area = "Admin" });
        }
    }
}
