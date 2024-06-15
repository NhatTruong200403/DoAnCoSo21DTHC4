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
    public class CarTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarTypesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/CarTypes
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.CarTypes.ToListAsync());
        //}
        public async Task<List<string?>> SearchSuggestions(string query)
        {
            return await _context.CarTypes

            .Where(p => p.Name.Contains(query))
            .Select(p => p.Name).Distinct()
            .ToListAsync();
        }

        public async Task<IActionResult> Index(string query, int pageNumber = 1)
        {
            int pageSize = _context.CarTypes.Count();
            IQueryable<CarType> Query;
            if (query != null)
            {
                Query = _context.CarTypes.Where(b => b.Name.Contains(query) && b.IsDeleted == false).Distinct();
            }
            else
            {
                Query = _context.CarTypes.Where(b => b.IsDeleted == false).Distinct();
            }
            var paginatedAmenities = await PaginatedList<CarType>.CreateAsync(Query, pageNumber, pageSize);
            return View(paginatedAmenities);
        }

        // GET: Admin/CarTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carType = await _context.CarTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carType == null)
            {
                return NotFound();
            }
            var carTypeCompanies = _context.CarTypesDetails
                .Where(c => c.CarTypeId == carType.Id)
                .Select(c => c.Company.Name)
                .ToList().Distinct();
            ViewData["CarTypeCompanies"] = carTypeCompanies;
            return View(carType);
        }

        // GET: Admin/CarTypes/Create
        public IActionResult Create()
        {
            var companies = _context.Companies.ToList();
            ViewData["Companies"] = new SelectList(companies, "Id", "Name");
            return View();
        }


        private async Task AddCarTypeDetailAsync(int carTypeId, int[] selectedCompanies)
        {
            foreach(var item in selectedCompanies)
            {
                var carTypeDetail = new CarTypeDetail()
                {
                    CarTypeId = carTypeId,
                    CompanyId = item
                };
                _context.CarTypesDetails.Add(carTypeDetail);
                await _context.SaveChangesAsync();
            }    
        }
        // POST: Admin/CarTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,IsDeleted")] CarType carType, int[] SelectedCompanies)
        {
            var user = await _userManager.GetUserAsync(User);
            carType.CreatedById = user!.Id;
            carType.CreatedOn = DateTime.Now;
            carType.ModifiedOn = null;
            if (ModelState.IsValid)
            {
                _context.Add(carType);
                await _context.SaveChangesAsync();
                await AddCarTypeDetailAsync(carType.Id, SelectedCompanies);
                return RedirectToAction(nameof(Index));
            }
            return View(carType);
        }

        // GET: Admin/CarTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carType = await _context.CarTypes.FindAsync(id);
            if (carType == null)
            {
                return NotFound();
            }
            var companies = _context.Companies.ToList();
            ViewData["Companies"] = new SelectList(companies, "Id", "Name");
            ViewData["CarTypeDetails"] = _context.CarTypesDetails
                                                    .Where(p => p.CarTypeId == id && p.IsDeleted == false)
                                                    .Select(p => p.Company.Name).ToList();

            return View(carType);
        }

        private async Task RemoveCarTypeDetailAsync(int carTypeId)
        {
            var carTypeDetailsToRemove = _context.CarTypesDetails.Where(p => p.CarTypeId == carTypeId).ToList();
            _context.CarTypesDetails.RemoveRange(carTypeDetailsToRemove);
            await _context.SaveChangesAsync();
        }
        public async Task<CarType?> GetExistingCarType(int id) 
                => await _context.CarTypes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        // POST: Admin/CarTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,IsDeleted")] CarType carType, int[] SelectedCompanies)
        {
            if (id != carType.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User); //Lấy thông tin user
            var existingCarType = await GetExistingCarType(id); // Lấy giá trị cũ của khuyến mãi
            carType.CreatedOn = existingCarType!.CreatedOn;
            carType.CreatedById = existingCarType.CreatedById;
            carType.ModifiedById = existingCarType!.ModifiedById;
            carType.ModifiedOn = existingCarType.ModifiedOn;

            var isChange = IsCarTypeDetailsChange(SelectedCompanies, carType.Id);
            if (isChange)
            {
                await RemoveCarTypeDetailAsync(carType.Id);
                await AddCarTypeDetailAsync(carType.Id, SelectedCompanies!);
                EditHelper<CarType>.SetModifiedIfNecessary(carType, true, existingCarType, user!.Id);

            }
            else
            {
                bool hasChanges = EditHelper<CarType>.HasChanges(carType, existingCarType);
                EditHelper<CarType>.SetModifiedIfNecessary(carType, hasChanges, existingCarType, user!.Id);
            }    
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarTypeExists(carType.Id))
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
            return View(carType);
        }

        private bool IsCarTypeDetailsChange(int[] SelectedCompanies, int carTypeId)
        {
            var companies = _context.Companies.ToList();
            var carTypeDetails = _context.CarTypesDetails
                                    .Where(p => p.CarTypeId == carTypeId)
                                    .Select(p => p.Company.Name).ToList();
            if (SelectedCompanies.Length > 0)
            {
                // Kiểm tra các checkbox đã chọn và có thay đổi không
                foreach (var item in companies)
                {
                    bool isChecked = carTypeDetails != null && carTypeDetails.Contains(item.Name);
                    bool currentChecked = SelectedCompanies != null && Array.IndexOf(SelectedCompanies, item.Id) != -1;

                    // Nếu trạng thái của checkbox đã thay đổi, đặt giá trị của biến bool là true
                    if (isChecked != currentChecked)
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        private bool CarTypeExists(int id)
        {
            return _context.CarTypes.Any(e => e.Id == id);
        }
    }
}
