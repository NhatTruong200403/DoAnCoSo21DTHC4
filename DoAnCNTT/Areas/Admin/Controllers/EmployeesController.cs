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

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: CustomerController
        public async Task<ActionResult> Index(bool showLockedAccounts = false)
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            if (showLockedAccounts)
            {
                employees = employees.Where(u => u.LockoutEnd > DateTime.Now).ToList();
            }
            ViewBag.IsFilteringLockedAccounts = showLockedAccounts;
            return View(employees);
        }

        public async Task<IActionResult> SearchEmployees(string query)
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            if (!string.IsNullOrEmpty(query))
            {
                employees = employees.Where(u => u.Email == query || u.PhoneNumber == query).ToList();
            }
            return View("Index", employees);
        }

        // GET: Admin/Reports/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // GET: Admin/Reports/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            return View();
        }

        public async Task<IActionResult> LockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (user.LockoutEnd < DateTime.Now || user.LockoutEnd == null)
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                    user.LockoutEnabled = true;
                }
                var result = await _userManager.UpdateAsync(user!);
            }
            return RedirectToAction("Index", "Employees", new { area = "Admin" });
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
            return RedirectToAction("Index", "Employees", new { area = "Admin" });
        }
    }
}
