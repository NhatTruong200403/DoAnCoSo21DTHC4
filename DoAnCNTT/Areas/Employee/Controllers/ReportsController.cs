﻿using System;
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

namespace DoAnCNTT.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employee/Reports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Report.Include(r => r.Post);
            return View(await applicationDbContext.ToListAsync());
        }

/*        // GET: Employee/Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .Include(r => r.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }*/

        // GET: Employee/Reports/Create
        public IActionResult Create(int id)
        {
            var report = new Report()
            {
                PostId = id
            };
            //ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            return View(report);
        }

        //Cập nhật lại số lần khách hàng vi phạm
        public async Task UpdateUserReportPoint(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.ReportPoint++;
                if(user.ReportPoint >= 3)
                {
                    user.LockoutEnd = DateTime.Now.AddYears(1000);
                }    
            }
            var result = await _userManager.UpdateAsync(user!);
        }

        // POST: Employee/Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,PostId")] Report report)
        {
            var user = await _userManager.GetUserAsync(User);
            report.CreatedOn = DateTime.Now;
            report.CreatedById = user!.Id;
            report.IsDeleted = false;

            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { area = "Customer", id = report.PostId });
            }
            return View(report);
        }
        /*
                // GET: Employee/Reports/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var report = await _context.Report.FindAsync(id);
                    if (report == null)
                    {
                        return NotFound();
                    }
                    ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", report.PostId);
                    return View(report);
                }

                // POST: Employee/Reports/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("Content,PostId,Id,CreatedById,CreatedOn,ModifiedById,ModifiedOn,IsDeleted")] Report report)
                {
                    if (id != report.Id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(report);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ReportExists(report.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", report.PostId);
                    return View(report);
                }

        */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .Include(r => r.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        //Xác nhận khóa bài đăng 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Report.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (report != null)
            {
                report.IsDeleted = true;
                report.ModifiedOn = DateTime.Now;
                report.ModifiedById = user!.Id;
                _context.Report.Update(report);
                var reportedPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == report.PostId);
                if (reportedPost != null)
                {
                    reportedPost.IsDisabled = true;
                    await ReturnBookedPost(reportedPost.Id);
                    await UpdateUserReportPoint(reportedPost.UserId!);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Hoàn tiền các hóa đơn cọc của bị bài đăng bị khóa
        public async Task ReturnBookedPost(int postId)
        {
            var postBookings = await _context.Booking.Where(b => b.PostId == postId && b.Status != "Hoàn tất").ToListAsync();
            foreach(var booking in postBookings)
            {
                booking.IsDeleted = true;
                booking.Status = "Đã trả cọc";
                var refundInvoice = new Invoice()
                {
                    Total = -(decimal)booking.PrePayment,
                    ReturnOn = DateTime.Now,
                    BookingId = booking.Id,
                    CreatedOn = DateTime.Now,
                };
                 _context.Invoices.Add(refundInvoice);
                await _context.SaveChangesAsync();
            }
        }

        private bool ReportExists(int id)
        {
            return _context.Report.Any(e => e.Id == id);
        }
    }
}
