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
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Companies.ToListAsync());
        //}
        public async Task<List<string?>> SearchSuggestions(string query)
        {
            return await _context.Companies

            .Where(p => p.Name.Contains(query))
            .Select(p => p.Name).Distinct()
            .ToListAsync();
        }
        public async Task<IActionResult> Index(string query, int pageNumber = 1)
        {
            int pageSize = 6;
            IQueryable<Company> Query;
            if (query != null)
            {
                Query = _context.Companies.Where(b => b.Name.Contains(query) && b.IsDeleted == false).Distinct();
            }
            else
            {
                Query = _context.Companies.Where(b => b.IsDeleted == false).Distinct();
            }
            var paginatedAmenities = await PaginatedList<Company>.CreateAsync(Query, pageNumber, pageSize);
            return View(paginatedAmenities);
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
            var companyCarTypes = _context.CarTypesDetails
                .Where(c => c.CompanyId == company.Id)
                .Select(c => c.CarType.Name)
                .ToList();
            ViewData["CompanyCarTypes"] = companyCarTypes;
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
            var carTypes = _context.CarTypes.ToList();
            ViewData["CarTypes"] = new SelectList(carTypes, "Id", "Name");
            return View();
        }

        private async Task AddCarTypeDetailAsync(int companyId, int[] SelectedCarTypes)
        {
            if (SelectedCarTypes != null)
            {
                foreach (var item in SelectedCarTypes)
                {
                    var carTypeDetail = new CarTypeDetail()
                    {
                        CarTypeId = item,
                        CompanyId = companyId
                    };
                    _context.CarTypesDetails.Add(carTypeDetail);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // POST: Admin/Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,IsDeleted")] Company company, IFormFile? IconImage, int[] SelectedCarTypes)
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
                await AddCarTypeDetailAsync(company.Id, SelectedCarTypes);
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
            var carTypes = _context.CarTypes.ToList();
            var carTypeDetails = _context.CarTypesDetails
                        .Where(p => p.CompanyId == id && p.IsDeleted == false)
                        .Select(p => p.CarType.Name).ToList();
            ViewData["CarTypes"] = new SelectList(carTypes, "Id", "Name");
            ViewData["CarTypeDetails"] = carTypeDetails;
            return View(company);
        }

        private async Task RemoveCarTypeDetailAsync(int companyId)
        {
            var carTypeDetailsToRemove = _context.CarTypesDetails.Where(p => p.CompanyId == companyId).ToList();
            _context.CarTypesDetails.RemoveRange(carTypeDetailsToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<Company?> GetExistingCompany(int id) 
                => await _context.Companies.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        // POST: Admin/Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,IsDeleted")] Company company, IFormFile? IconImage, int[] SelectedCarTypes)
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
                    var isChange = IsCarTypeDetailsChange(SelectedCarTypes, company.Id);
                    if (isChange)
                    {
                        await RemoveCarTypeDetailAsync(company.Id);
                        await AddCarTypeDetailAsync(company.Id, SelectedCarTypes!);
                        EditHelper<Company>.SetModifiedIfNecessary(company, true, existingCompany, user!.Id);

                    }
                    else
                    {
                        bool hasChanges = EditHelper<Company>.HasChanges(company, existingCompany);
                        EditHelper<Company>.SetModifiedIfNecessary(company, hasChanges, existingCompany, user!.Id);
                    }
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

        private bool IsCarTypeDetailsChange(int[] SelectedCarTypes, int companyId)
        {
            var carTypes = _context.CarTypes.ToList();
            var carTypeDetails = _context.CarTypesDetails
                                    .Where(p => p.CompanyId == companyId && p.IsDeleted == false)
                                    .Select(p => p.CarType.Name).ToList();
            if (SelectedCarTypes.Length > 0)
            {
                // Kiểm tra các checkbox đã chọn và có thay đổi không
                foreach (var item in carTypes)
                {
                    bool isChecked = carTypeDetails != null && carTypeDetails.Contains(item.Name);
                    bool currentChecked = SelectedCarTypes != null && Array.IndexOf(SelectedCarTypes, item.Id) != -1;

                    // Nếu trạng thái của checkbox đã thay đổi, đặt giá trị của biến bool là true
                    if (isChecked != currentChecked)
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
