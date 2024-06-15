using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using DoAnCNTT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Mono.TextTemplating;

namespace DoAnCNTT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> FindUser(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(id);
            if(currentUser !=  null)
            {
                var userBooking = await _context.Booking.
                                    Where(b => b.UserId == currentUser!.Id && b.Post.UserId == user!.Id).
                                    ToListAsync();
                ViewBag.userBooking = userBooking;
            }
            var listpost = await _context.Posts
                                         .Where(p => p.CreatedById == id)
                                         .ToListAsync();
            ViewBag.listpost = listpost;
            return PartialView(user);
        }

        public void UpdateExpiredPromotion()
        {
            var expiredPromotions = _context.Promotions.Where(p => p.ExpiredDate <= DateTime.Now).ToList();
            foreach(var item in expiredPromotions)
            {
                item.IsDeleted = true;
                _context.Promotions.Update(item);
                _context.SaveChanges(); 
            }    
        }

        public async Task<List<string?>> SearchSuggestions(string query)
        {
            return await _context.Posts

            .Where(p => p.Name!.Contains(query))
            .Select(p => p.Name).Distinct()
            .ToListAsync();
        }

        public async Task<IActionResult> Index(string company, int seat, string gear, string fuel, bool hasDriver, int pageNumber = 1)
        {
            int pageSize = 8;
            IQueryable<Post> carsQuery = _context.Posts.Where(b => !b.IsDeleted && !b.IsDisabled).OrderByDescending(p=>p.AvgRating).Take(8);
            return View(carsQuery);
        }


        public async Task<IActionResult> ViewSearch(string company, int seat, string gear, string fuel,bool hasDriver, int pageNumber = 1)
        {
            int pageSize = 8;
            IQueryable<Post> carsQuery = _context.Posts.Where(b => !b.IsDeleted && !b.IsDisabled);
            if (!string.IsNullOrEmpty(company))
            {
                carsQuery = carsQuery.Where(b => b.Company.Name == company);
            }
            if (seat > 0)
            {
                carsQuery = carsQuery.Where(b => b.Seat == seat);
            }
            if (!string.IsNullOrEmpty(gear))
            {
                if(gear == "Số sàn")
                {
                    carsQuery = carsQuery.Where(b => b.Gear == false);
                }
                else
                {
                    carsQuery = carsQuery.Where(b => b.Gear == true);
                }    
            }  
            if(!string.IsNullOrEmpty(fuel))
            {
                if (fuel == "Xăng")
                {
                    carsQuery = carsQuery.Where(b => b.Fuel == "Xăng");
                }
                else if(fuel == "Điện")
                {
                    carsQuery = carsQuery.Where(b => b.Fuel == "Điện");
                }
                else
                {
                    carsQuery = carsQuery.Where(b => b.Fuel == "Dầu");
                }    
            }    
            if(hasDriver)
            {
                
                carsQuery = carsQuery.Where(b => b.HasDriver == true);
            }    
            var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery.Distinct(), pageNumber, pageSize);
            ViewData["Company"] = company;
            ViewData["Seat"] = seat;
            ViewData["Gear"] = gear;
            ViewData["HasDriver"] = hasDriver;
            // Call UpdateExpiredPromotion method (assuming it's a void method)
            UpdateExpiredPromotion();

            // Return the view with paginated results
            return View(paginatedCar);
        }

        public async Task<IActionResult> SortByPrice(bool? sortPriceOrder)
        {
            bool isSortAscending = sortPriceOrder ?? true;
            IQueryable<Post> sortedList = _context.Posts.Where(b => !b.IsDeleted && !b.IsDisabled);
            var paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderBy(p => p.Price), 1, 6);
            if (!isSortAscending)
            {
                paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderByDescending(p => p.Price), 1, 6);
            }
            ViewBag.SortPriceOrder = isSortAscending;
            return View("ViewSearch", paginatedCar);
        }

        public async Task<IActionResult> SortByRideNumber(bool? sortRideNumberOrder)
        {
            bool isSortAscending = sortRideNumberOrder ?? true;
            IQueryable<Post> sortedList = _context.Posts.Where(b => !b.IsDeleted && !b.IsDisabled);
            var paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderBy(p => p.RideNumber), 1, 6);
            if (!isSortAscending)
            {
                paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderByDescending(p => p.RideNumber), 1, 6);
            }
            ViewBag.SortRideNumberOrder = isSortAscending;
            return View("ViewSearch", paginatedCar);
        }

        public async Task<IActionResult> SortByRating(bool? sortRatingOrder)
        {
            bool isSortAscending = sortRatingOrder ?? true;
            IQueryable<Post> sortedList = _context.Posts.Where(b => !b.IsDeleted && !b.IsDisabled);
            var paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderBy(p => p.AvgRating), 1, 6);
            if (!isSortAscending)
            {
                paginatedCar = await PaginatedList<Post>.CreateAsync(sortedList.OrderByDescending(p => p.AvgRating), 1, 6);
            }
            ViewBag.SortRatingOrder = isSortAscending;
            return View("ViewSearch", paginatedCar);
        }

        [HttpGet]
        public async Task<IActionResult> GetCarCompanies()
        {
            var companies = await _context.Companies.Select(c => c.Name).Distinct().ToListAsync();
            return Json(companies);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartypes(string name, int pageNumber = 1)
        {
            int pageSize = 8;
            IQueryable<Post> carsQuery = _context.Posts
                .Where(p => p.CarType.Name == name && !p.IsDeleted && !p.IsDisabled);

            var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery.Distinct(), pageNumber, pageSize);

            return View("ViewSearch", paginatedCar);
        }
        [HttpGet]
        public async Task<IActionResult> Address(string name, int pageNumber = 1)
        {
            int pageSize = 8;
            IQueryable<Post> carsQuery = _context.Posts.Where(p => p.RentLocation.Contains(name) && !p.IsDeleted && !p.IsDisabled);
            var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery.Distinct(), pageNumber, pageSize);
            return View("ViewSearch", paginatedCar);
        }


        [HttpGet]
        public async Task<IActionResult> SumCars(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên địa phương không hợp lệ.");
            }

            try
            {
                var sum = await _context.Posts
                    .Where(p => p.RentLocation == name && !p.IsDeleted && !p.IsDisabled)
                    .CountAsync(); // Lấy tổng số xe
                return Json(sum);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                // Return a user-friendly error message
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
            }
        }









        public IActionResult ChinhSach()
        {
            return View();
        }
        public IActionResult Dieukhoan()
        {
            return View();
        }
        public IActionResult GioiThieu()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


//public async Task<IActionResult> Index(string query, int pageNumber = 1)
//{
//    int pageSize = 6;
//    IQueryable<Post> carsQuery;
//    if (query != null)
//    {
//        carsQuery = _context.Posts.Where(b => b.Name.Contains(query) && b.IsDeleted == false).Distinct();
//    }
//    else
//    {
//        carsQuery = _context.Posts.Where(b => b.IsDeleted == false).Distinct();
//    }
//    var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery, pageNumber, pageSize);
//    ViewData["SearchString"] = query;
//    ViewData["Companies"] = _context.Posts.Select(f => f.Company).ToList();
//    ViewData["CarTypes"] = _context.Posts.Select(f => f.CarType).ToList();
//    UpdateExpiredPromotion();
//    return View(paginatedCar);
//}