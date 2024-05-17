using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using DoAnCNTT.Data;

namespace DoAnCNTT.Controllers
{
    //TestPush
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;
        

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            //_userManager = userManager;
            _context = context;
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

        public IActionResult Index()
        {
            var post = _context.Posts.ToList();
            UpdateExpiredPromotion();
            return View(post);
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
