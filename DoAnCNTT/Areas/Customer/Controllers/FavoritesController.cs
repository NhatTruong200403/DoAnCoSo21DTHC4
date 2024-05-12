using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> AddToFavorite(int id, string userId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return BadRequest("Khong tim thay bai dang");
            }
            var excistingFavorite = await _context.Favorite.FirstOrDefaultAsync(p => p.PostId == id && p.UserId == userId);
            if (excistingFavorite == null)
            {
                var Favorite = new Favorite
                {   UserId = userId,
                    PostId = id
                };
                _context.Favorite.Add(Favorite);
                _context.SaveChanges();
                return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
            }
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
        }
        public IActionResult Detail(int id)
        {
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
        }
        public IActionResult FavoriteList(string userId)
        {
            ViewData["Car"] = _context.Favorite.Select(p=>p.Post.Name).ToList();
            ViewData["Image"] = _context.Favorite.Select(p => p.Post.Image).ToList();
            ViewData["CarType"] = _context.Favorite.Select(p => p.Post.CarType.Name).ToList();
            ViewData["Company"] = _context.Favorite.Select(p=> p.Post.Company.Name).ToList();
            var Favorite = _context.Favorite.Where(p=>p.UserId == userId).ToList();
            return View(Favorite);
        }
        [HttpGet]
        public async Task<IActionResult> RemoveFromFavoriteList(int? id, string userId)
        {
            if(userId == null || id == null)
            {
                return NotFound();
            }
            var FavoriteList = await _context.Favorite.Where(p=>p.PostId == id && p.UserId == userId).FirstOrDefaultAsync();
            if(FavoriteList == null)
            {
                return NotFound();
            }
            _context.Remove(FavoriteList);
            _context.SaveChanges();
            return RedirectToAction("FavoriteList", new {userId = userId});
        }
    }
}
