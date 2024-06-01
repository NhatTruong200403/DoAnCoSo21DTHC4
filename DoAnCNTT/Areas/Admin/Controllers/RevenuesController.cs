using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using DoAnCNTT.Models.ViewModel;

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RevenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RevenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Revenues
        public async Task<IActionResult> Index(int day, int month)
        {
            var invoices = await CalculateRevenues(day, month);
            var revenues = invoices.Any() ? invoices.Sum(i => i.Total) : 0;

            var revenuesVM = new RevenuesViewModel
            {
                Invoices = invoices,
                SelectedDay = day,
                SelectedMonth = month,
                Revenues = revenues
            };

            return View(revenuesVM);
        }

        // POST: Admin/Revenues
        [HttpPost]
        public IActionResult Index(RevenuesViewModel model)
        {
            return RedirectToAction("Index", new { day = model.SelectedDay, month = model.SelectedMonth });
        }

        private async Task<List<Invoice>> CalculateRevenues(int day, int month)
        {
            var invoices = await _context.Invoices
                            .Include(i => i.Booking)
                            .ThenInclude(b => b.User)
                            .ToListAsync();

            if (day > 0 && month > 0)
            {
                invoices = invoices.Where(i => i.CreatedOn.Day == day && i.CreatedOn.Month == month).ToList();
            }
            else if (day == 0 && month > 0)
            {
                invoices = invoices.Where(i => i.CreatedOn.Month == month).ToList();
            }
            else if (day > 0 && month == 0)
            {
                invoices = invoices.Where(i => i.CreatedOn.Day == day && i.CreatedOn.Month == DateTime.Now.Month).ToList();
            }

            return invoices;
        }
    }
}
