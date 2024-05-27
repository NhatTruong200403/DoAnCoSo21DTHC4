using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using DoAnCNTT.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> FindUser(string id, int idpost)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(id);
            var userBooking = await _context.Booking.Where(b => b.UserId == currentUser!.Id && b.PostId == idpost).ToListAsync();


            var listpost = await _context.Posts
                                         .Where(p => p.CreatedById == id)
                                         .ToListAsync();
            ViewBag.listpost = listpost;
            ViewBag.userBooking = userBooking;

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

        public async Task<IActionResult> Index(string query, int pageNumber = 1)
        {
            int pageSize = 6;
            IQueryable<Post> carsQuery;
            if (query != null)
            {
                carsQuery = _context.Posts.Where(b => b.Name!.Contains(query) && b.IsDeleted == false).Distinct();
            }
            else
            {
                carsQuery = _context.Posts.Where(b => b.IsDeleted == false).Distinct();
            }
            var paginatedCar = await PaginatedList<Post>.CreateAsync(carsQuery, pageNumber, pageSize);
            ViewData["SearchString"] = query;
            ViewData["Companies"] = _context.Posts.Select(f => f.Company).ToList();
            ViewData["CarTypes"] = _context.Posts.Select(f => f.CarType).ToList();
            UpdateExpiredPromotion();
            return View(paginatedCar);
        }

        //public IActionResult Index()
        //{

        //    var post = _context.Posts.ToList();
            
        //    return View(post);
        //}

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
