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
using DoAnCNTT.Models.Utilities;

namespace DoAnCNTT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        // GET: Admin/Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            var savePath = "./wwwroot/images/companies/";
            var filePath = Path.Combine(savePath + file?.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file!.CopyToAsync(fileStream);
            }
            return "/images/companies/" + file.FileName;
        }

        // GET: Admin/Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,IsDeleted")] Company company, IFormFile? IconImage)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                company.CreatedById = user!.Id;
                company.CreatedOn = DateTime.Now;
                if (IconImage != null)
                {
                    company.IconImage = await SaveImage(IconImage);
                }
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Admin/Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        public async Task<Company?> GetExistingCompany(int id) 
                => await _context.Companies.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        // POST: Admin/Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,IsDeleted")] Company company, IFormFile? IconImage)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var existingCompany = await GetExistingCompany(id);
                    if (IconImage != null)
                    {
                        company.IconImage = await SaveImage(IconImage);
                    }
                    else
                    {
                        company.IconImage = existingCompany!.IconImage;
                    }
                    company.CreatedOn = existingCompany!.CreatedOn;
                    company.CreatedById = existingCompany.CreatedById;
                    company.ModifiedOn = existingCompany.ModifiedOn;
                    company.ModifiedById = existingCompany.ModifiedById;
                    bool hasChanges = EditHelper<Company>.HasChanges(company, existingCompany); //Hàm kiểm tra
                    EditHelper<Company>.SetModifiedIfNecessary(company, hasChanges, existingCompany, user!.Id); //Hàm cập nhật nếu thay đổi
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
