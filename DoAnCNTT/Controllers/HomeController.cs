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

            .Where(p => p.Name.Contains(query))
            .Select(p => p.Name).Distinct()
            .ToListAsync();
        }



        public async Task<IActionResult> Index(string query, string company, int seat, int pageNumber = 1)
        {
            int pageSize = 6;
            IQueryable<Post> carsQuery = _context.Posts.Where(b => b.IsDeleted == false);
            if (!string.IsNullOrEmpty(query))
            {
                carsQuery = carsQuery.Where(b => b.Name.Contains(query));

            }
            if (!string.IsNullOrEmpty(company))
            {
                carsQuery = carsQuery.Where(b => b.Company.Name == company);
            }
            if (seat>0)
            {
                carsQuery = carsQuery.Where(b => b.Seat == seat);
            }
            var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery.Distinct(), pageNumber, pageSize);
            ViewData["SearchString"] = query;
            ViewData["Companies"] = await _context.Companies.Select(c => c.Name).Distinct().ToListAsync();
            ViewData["Seats"] = await _context.Posts.Select(p => p.Seat.ToString()).Distinct().ToListAsync();

            // Call UpdateExpiredPromotion method (assuming it's a void method)
            UpdateExpiredPromotion();

            // Return the view with paginated results
            return View(paginatedCar);
        }



        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _context.Companies.Select(c => c.Name).Distinct().ToListAsync();
            return Json(companies);
        }

        [HttpGet]
        public async Task<IActionResult> GetSeats()
        {
            var seats = await _context.Posts.Select(p => p.Seat).Distinct().ToListAsync();
            return Json(seats);
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