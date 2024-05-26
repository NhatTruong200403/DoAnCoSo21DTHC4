using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using DoAnCNTT.Payment.Momo;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
    public class InvoicesController : Controller
    {


        // GET: Customer/Invoices/Create
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public InvoicesController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }

        // GET: Customer/Invoices
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var invoices = await _context.Invoices.Include(i => i.Booking).Where(i => i.Booking.UserId == user!.Id).ToListAsync();
            return View(invoices);
        }


        // GET: Customer/Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Customer/Invoices/Details/5
        public async Task<IActionResult> MomoPayment(int bookingId)
        {
            var booking = await _context.Booking.FindAsync(bookingId);
            if (booking == null || booking.IsDeleted)
            {
                ViewBag.No = "Không tìm thấy đơn đặt xe";
                return RedirectToAction("Index", "Bookings");
            }

            double price = (double)booking.PrePayment;
            string priceStr = price.ToString();

            int newBookingId = booking.Id;
            string newBookingIdStr = newBookingId.ToString();

            //string endpoint = _configuration.GetValue<string>("MomoAPI:MomoApiUrl");
            //string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey");
            //string accessKey = _configuration.GetValue<string>("MomoAPI:AccessKey");
            //string returnUrl = _configuration.GetValue<string>("MomoAPI:ReturnUrl");
            //string notifyUrl = _configuration.GetValue<string>("MomoAPI:NotifyUrl");
            //string partnerCode = _configuration.GetValue<string>("MomoAPI:PartnerCode");
            string endpoint = _configuration.GetValue<string>("MomoAPI:MomoApiUrl") ?? string.Empty;
            string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey") ?? string.Empty;
            string accessKey = _configuration.GetValue<string>("MomoAPI:AccessKey") ?? string.Empty;
            string returnUrl = _configuration.GetValue<string>("MomoAPI:ReturnUrl") ?? string.Empty;
            string notifyUrl = _configuration.GetValue<string>("MomoAPI:NotifyUrl") ?? string.Empty;
            string partnerCode = _configuration.GetValue<string>("MomoAPI:PartnerCode") ?? string.Empty;

            string orderInfo = newBookingIdStr;
            string amount = priceStr;
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyUrl + "&extraData=" +
                extraData;

            MomoSecurity crypto = new();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new()
            {
                      { "partnerCode", partnerCode },
                      { "accessKey", accessKey },
                      { "requestId", requestId },
                      { "amount", amount },
                      { "orderId", orderid },
                      { "orderInfo", orderInfo },
                      { "returnUrl", returnUrl },
                      { "notifyUrl", notifyUrl },
                      { "extraData", extraData },
                      { "requestType", "captureMoMoWallet" },
                      { "signature", signature }

                };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            var payUrlToken = jmessage.GetValue("payUrl");
            if (payUrlToken != null)
            {
                string payUrl = payUrlToken.ToString();
                if (!string.IsNullOrEmpty(payUrl))
                {
                    return Redirect(payUrl);
                }
            }
            // sửa
            return Ok(responseFromMomo);
            // Handle the case where payUrl is null or empty
        }
        public async Task<IActionResult> ReturnUrl()
        {
            string param = "";
            param += "partnerCode=" + Request.Query["partnerCode"];
            param += "&accessKey=" + Request.Query["accessKey"];
            param += "&requestId=" + Request.Query["requestId"];
            param += "&amount=" + Request.Query["amount"];
            param += "&orderId=" + Request.Query["orderId"];
            param += "&orderInfo=" + Request.Query["orderInfo"];
            param += "&orderType=" + Request.Query["orderType"];
            param += "&transId=" + Request.Query["transId"];
            param += "&message=" + Request.Query["message"];
            param += "&localMessage=" + Request.Query["localMessage"];
            param += "&responseTime=" + Request.Query["responseTime"];
            param += "&errorCode=" + Request.Query["errorCode"];
            param += "&payType=" + Request.Query["payType"];
            param += "&extraData=" + Request.Query["extraData"];

            string bookingIdStr = Request.Query["orderInfo"].ToString() ?? ""; // Providing an empty string as default
            int bookingIdNum = int.Parse(bookingIdStr);
            var booking = await _context.Booking.FindAsync(bookingIdNum);
            //param = Server.UrlEncode(param);
            MomoSecurity crypto = new();
            //string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey");
            string serectkey = _configuration.GetValue<string>("MomoAPI:Serectkey") ?? ""; // Providing an empty string as default
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request.Query["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hợp lệ";
                return View();
            }
            if (!Request.Query["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thất bại";
                booking!.IsDeleted = true;
                _context.Booking.Update(booking);
                _context.SaveChanges();
                await UpdatePostInfo(booking, true, -1);
                return RedirectToAction("Details", "Posts", new { id = booking.PostId }); ;
            }
            else
            {
                if (booking != null)
                {
                    booking.IsPay = true;
                    var invoice = new Invoice()
                    {
                        Total = -booking.PrePayment,
                        ReturnOn = booking.RecieveOn.AddDays(2),
                        BookingId = booking.Id,
                        CreatedOn = booking.CreatedOn,
                    };
                    _context.Invoices.Add(invoice);
                    _context.SaveChanges();
                    var isAvailable = false;
                    if (booking.RecieveOn > DateTime.Now)
                    {
                        isAvailable = true;
                    }
                    await UpdatePostInfo(booking, isAvailable, 1);

                }

            }
            return RedirectToAction(nameof(Index));


        }

        private async Task UpdatePostInfo(Booking? booking, bool isAvailable, int rideNumber)
        {
            var post = await _context.Posts.FindAsync(booking!.PostId);
            if (post != null)
            {
                post.IsAvailable = isAvailable;
                if (post.RideNumber == 0 && rideNumber < 0)
                {
                    post.RideNumber = 0;
                }
                else
                {
                    post.RideNumber += rideNumber;
                }
            }
            _context.SaveChanges();
        }
    }
}
