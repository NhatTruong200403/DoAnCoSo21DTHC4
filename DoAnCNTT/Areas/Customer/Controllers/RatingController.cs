using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public RatingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //Thêm comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([Bind("PostId,Comment,Point,CreatedById,UserId")] Rating rating)
        {
            if (rating == null)
            {
                return BadRequest("Phai nhap cai gi do");
            }
            if (ModelState.IsValid)
            {
                rating.CreatedOn = DateTime.Now;
                _context.Add(rating);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = rating.PostId });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateComment(int id, [Bind("Id, Comment, Point")] Rating rating)
        //{
        //    if (id != rating.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var existingRating = await _context.Ratings.FindAsync(id);
        //            if (existingRating == null)
        //            {
        //                return NotFound();
        //            }

        //            existingRating.Comment = rating.Comment;
        //            existingRating.Point = rating.Point;

        //            _context.Update(existingRating);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!RatingExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //    return View();
        //}

        public async Task<IActionResult> Delete(int id, int postId)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { area = "Customer", id = postId}); ;
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }

    }
}




