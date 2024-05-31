using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

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
        public async Task<IActionResult> IndexAsync(int day, int month)
        {
            var invoices = await CalculateRevenues(day, month);
            var revenues = invoices.Any() ? invoices.Sum(i => i.Total) : 0;
            ViewData["revenues"] = revenues;
            return View(invoices);
        }


        public async Task<List<Invoice>> CalculateRevenues(int day, int month)
        {
            var invoices = await _context.Invoices
                            .Include(i => i.Booking)
                            .ThenInclude(b => b.User)
                            .Where(i => i.CreatedOn.Day == DateTime.Now.Day && i.CreatedOn.Month == DateTime.Now.Month)
                            .ToListAsync();

            if (day > 0 && month > 0)
            {
                invoices = await _context.Invoices
                            .Include(i => i.Booking)
                            .ThenInclude(b => b.User)
                            .Where(i => i.CreatedOn.Day == day && i.CreatedOn.Month == month)
                            .ToListAsync();
            }
            if(day == 0 && month > 0)
            {
                invoices = await _context.Invoices
                        .Include(i => i.Booking)
                        .ThenInclude(b => b.User)
                        .Where(i => i.CreatedOn.Month == month)
                        .ToListAsync();
            }
            if (day > 0 && month == 0)
            {
                invoices = await _context.Invoices
                        .Include(i => i.Booking)
                        .ThenInclude(b => b.User)
                        .Where(i => i.CreatedOn.Day == day && i.CreatedOn.Month == DateTime.Now.Month)
                        .ToListAsync();
            }
            return invoices;
        }
    }
}
