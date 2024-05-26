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
        public async Task<IActionResult> Index()
        {
            var date = DateTime.Now.Date;
            var totalRevenue = await _context.Invoices.Where(p => p.CreatedOn.Date == date).SumAsync(i => i.Total);
            ViewBag.TotalRevenue = totalRevenue;
            var allRevenue = await _context.Invoices.SumAsync(i=> i.Total);
            ViewBag.AllRevenue = allRevenue;
            var month = DateTime.Now.Month;
            var monthRevenues = await _context.Invoices.Where(p=>p.CreatedOn.Month == month).SumAsync(i=> i.Total);
            ViewBag.MonthRevenue = monthRevenues;
            return View();
        }
    }
}
