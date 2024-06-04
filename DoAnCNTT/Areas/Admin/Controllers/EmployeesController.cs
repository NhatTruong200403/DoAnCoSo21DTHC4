using DoAnCNTT.Data;
using DoAnCNTT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoAnCNTT.Areas.Admin.Controllers
{
	public class EmployeesController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public EmployeesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		// GET: CustomerController
		public async Task<ActionResult> Index()
		{
			var employee = await _userManager.GetUsersInRoleAsync("Employee");
			return View(employee);
		}
	}
}
