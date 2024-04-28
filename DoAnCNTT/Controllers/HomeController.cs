using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace DoAnCNTT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly UserManager<ApplicationUser> _userManager;
        

        public HomeController(ILogger<HomeController> logger/*, UserManager<ApplicationUser> userManager*/)
        {
            _logger = logger;
            //_userManager = userManager;
        }


//        [HttpGet]
//        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
//        {
//            if (remoteError != null)
//            {
//                // Xử lý lỗi từ Google (nếu có)
//                return RedirectToAction("Login"); // Hoặc chuyển hướng đến trang lỗi
//            }

//            // Nhận thông tin xác thực từ Google
//            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
//            if (info == null)
//            {
//                // Xử lý lỗi không thể xác thực
//                return RedirectToAction("Login"); // Hoặc chuyển hướng đến trang lỗi
//            }

//            // Lấy thông tin người dùng từ Google
//            var externalUserId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
//            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
//            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

//            // Kiểm tra xem người dùng đã đăng nhập chưa
//            var user = await _userManager.FindByLoginAsync("Google", externalUserId);
//            if (user == null)
//            {
//                // Nếu người dùng chưa tồn tại, tạo người dùng mới
//                user = new ApplicationUser { UserName = email, Email = email };
//                var result = await _userManager.CreateAsync(user);
//                if (!result.Succeeded)
//                {
//                    // Xử lý lỗi khi tạo người dùng mới
//                    return RedirectToAction("Login"); // Hoặc chuyển hướng đến trang lỗi
//                }

//                // Liên kết người dùng mới với tài khoản Google
//                result = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", externalUserId, "Google"));
//                if (!result.Succeeded)
//                {
//                    // Xử lý lỗi khi liên kết tài khoản Google
//                    return RedirectToAction("Login"); // Hoặc chuyển hướng đến trang lỗi
//                }
//            }

//            var claimsIdentity = new ClaimsIdentity(new List<Claim>
//{
//            new Claim(ClaimTypes.NameIdentifier, user.Id),
//            new Claim(ClaimTypes.Email, user.Email),
//            // Thêm các claim khác nếu cần thiết
//            }, IdentityConstants.ApplicationScheme);

//            // Tạo ClaimsPrincipal từ ClaimsIdentity
//            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

//            // Đăng nhập người dùng vào ứng dụng
//            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);

//            // Chuyển hướng đến trang mục tiêu sau khi đăng nhập thành công
//            return RedirectToAction("Index", "Home");
//        }

//        [HttpPost]
//        public async Task<IActionResult> ExternalLoginCallbackFromForm()
//        {
//            return await ExternalLoginCallback();
//        }

        public IActionResult Index()
        {
            return View();
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
