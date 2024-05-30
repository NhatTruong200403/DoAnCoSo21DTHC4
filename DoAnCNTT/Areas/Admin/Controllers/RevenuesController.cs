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
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CalculateRevenues(int day, int month)
        {
            var invoices = await _context.Invoices
                        .Where(i => i.CreatedOn.Day == DateTime.Now.Day && i.CreatedOn.Month == DateTime.Now.Month)
                        .Select(i => i.Total)
                        .ToListAsync();
            if (day > 0 && month > 0)
            {
                invoices = await _context.Invoices
                             .Where(i => i.CreatedOn.Day == day && i.CreatedOn.Month == month)
                             .Select(i => i.Total)
                             .ToListAsync();
            }
            if(day == 0 && month > 0)
            {
                invoices = await _context.Invoices
                     .Where(i => i.CreatedOn.Month == month)
                     .Select(i => i.Total)
                     .ToListAsync();
            }

            var revenues = invoices.Any() ? invoices.Average() : 0;

            return View("Index", revenues);
        }
    }
}
