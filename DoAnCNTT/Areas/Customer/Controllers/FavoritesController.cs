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
using Microsoft.AspNetCore.Authorization;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer")]
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
            var existingFavorite = await _context.Favorite.FirstOrDefaultAsync(p => p.PostId == id && p.UserId == userId);
            if (existingFavorite == null)
            {
                var Favorite = new Favorite
                {
                    UserId = userId,
                    PostId = id
                };
                _context.Favorite.Add(Favorite);
                _context.SaveChanges();
                return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
            }
            existingFavorite.IsDeleted = false;
            _context.Favorite.Update(existingFavorite);
            _context.SaveChanges();
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
        }
        public IActionResult Detail(int id)
        {
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = id });
        }
        public IActionResult FavoriteList(string userId)
        {
            var favorites = _context.Favorite
                                .Include(f => f.Post)
                                .ThenInclude(p => p.CarType)
                                .Include(f => f.Post)
                                .ThenInclude(p => p.Company)
                                .Where(f => f.UserId == userId && f.IsDeleted == false)
                                .ToList();
            return View(favorites);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavoriteList(int id, string userId)
        {
            if (userId == null || id == null)
            {
                return NotFound();
            }
            var favoritePost = await _context.Favorite.Where(p => p.PostId == id && p.UserId == userId).FirstOrDefaultAsync();
            if (favoritePost == null)
            {
                return NotFound();
            }
            favoritePost.IsDeleted = true;
            _context.Favorite.Update(favoritePost);
            _context.SaveChanges();
            return RedirectToAction("FavoriteList", new { userId = userId });
        }
    }
}
