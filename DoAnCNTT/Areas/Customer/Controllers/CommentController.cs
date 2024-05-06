using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> AddComment(int id, [Bind("Comment,Point")] Rating rating)
        {
            if (rating == null)
            {
                return BadRequest("Phai nhap cai gi do");
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                rating.CreatedOn = DateTime.Now;
                rating.CreatedById = user!.Id;
                rating.PostId = id;
                _context.Add(rating);
                await _context.SaveChangesAsync();
            }
            return PartialView(rating);
        }
    }
}
