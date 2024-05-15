using DoAnCNTT.Data;
using DoAnCNTT.Models;
using DoAnCNTT.Models.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNTT.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer/Posts
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var posts = _context.Posts.
                Include(p => p.CarType).
                Include(p => p.Company).
                Include(p => p.User).
                Where(p => p.IsDeleted == false && p.UserId == user!.Id).ToListAsync();
            return View(await posts);
        }

        public async Task IsPostAvailable(Post post)
        {
            var booking = await _context.Booking
                        .Where(b => b.PostId == post.Id)
                        .OrderByDescending(b => b.Id)
                        .FirstOrDefaultAsync();
            if (booking != null)
            {
                if (booking.ReturnOn <= DateTime.Now)
                {
                    post.IsAvailable = true;
                }
                else
                {
                    post.IsAvailable = false;
                }
            }
            else
            {
                post.IsAvailable = true;
            }
            _context.Posts.Update(post);
            _context.SaveChanges();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.CarType)
                .Include(p => p.Company)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            await IsPostAvailable(post);
            ViewData["PostAmenities"] = _context.PostAmenities.Include(pa => pa.Amenity).Where(p => p.PostId == id && post.IsDeleted == false).Select(p => p.Amenity).ToList();
            ViewData["PostIMG"] = _context.Amenities.ToList();
            ViewData["PostImages"] = _context.PostImages.Where(p => p.PostId == id).Select(p => p.Url).ToList();
            ViewData["Comment"] = _context.Ratings.Where(p => p.PostId == id).Select(p => p.Comment).ToList();
            ViewData["Star"] = _context.Ratings.Where(p => p.PostId == id).Select(p => p.Point).ToList();
            ViewData["Cmt"] = _context.Ratings.Where(p => p.PostId == id).ToList();
            var promotions = _context.Promotions.Where(p => p.IsDeleted == false).ToList();
            ViewData["Promotions"] = new SelectList(promotions, "Id", "Content");
            return View(post);
        }

        // GET: Customer/Posts/Create
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            var carTypes = _context.CarTypes.Where(c => c.IsDeleted == false).ToList();
            var amenities = _context.Amenities.Where(a => a.IsDeleted == false).ToList();
            ViewData["CarTypeId"] = new SelectList(carTypes, "Id", "Name");
            ViewData["Amenities"] = new SelectList(amenities, "Id", "Name");
            return View();
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            var savePath = "./wwwroot/images/post/";
            var filePath = Path.Combine(savePath + file?.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file!.CopyToAsync(fileStream);
            }
            return "/images/post/" + file.FileName;
        }

        public IActionResult GetCompanyCarType(int carTypeId)
        {
            var companies = _context.CarTypesDetails
                            .Where(c => c.CarTypeId == carTypeId && c.Company.IsDeleted == false)
                            .Select(c => c.Company)
                            .ToList().Distinct();
            // Tạo danh sách SelectListItem từ dữ liệu companies
            var companyItems = companies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            // Thêm một option mặc định vào đầu danh sách
            companyItems.Insert(0, new SelectListItem { Value = "", Text = "-- Chọn hãng xe --" });

            // Đặt danh sách vào ViewBag
            ViewBag.Companies = companyItems;

            return Json(companies);
        }
        public async Task SavePostAmenitiesAsync(int postId, int[] selectedAmenities)
        {
            foreach (var item in selectedAmenities)
            {
                var postAmenities = new PostAmenity()
                {
                    AmenityId = item,
                    PostId = postId
                };
                _context.PostAmenities.Add(postAmenities);
                await _context.SaveChangesAsync();
            }
        }


        private async Task SavePostImage(Post post, List<IFormFile> Images)
        {
            post.Images = new List<PostImages>();
            if (Images != null)
            {
                foreach (var image in Images)
                {
                    if (image != null && image.Length > 0)
                    {
                        var postImage = new PostImages();
                        postImage.Url = await SaveImage(image);
                        post.Images.Add(postImage);
                        _context.PostImages.Add(postImage);
                    }
                }
            }
        }

        // POST: Customer/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([Bind("Name,Description,Seat,RentLocation,HasDriver,Price,Fuel,FuelConsumed,CarTypeId,CompanyId,Id")] Post post, string Gear, IFormFile? Image, List<IFormFile> Images, int[] SelectedAmenities)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                post.CreatedById = user!.Id;
                post.UserId = user.Id;
                post.CreatedOn = DateTime.Now;
                post.ModifiedOn = null;
                post.IsDeleted = false;
                post.IsAvailable = true;
                post.RideNumber = 0;
                if (Gear == "Số tự động")
                {
                    post.Gear = true;
                }
                else
                {
                    post.Gear = false;
                }
                if (Image != null)
                {
                    post.Image = await SaveImage(Image);
                }
                await SavePostImage(post, Images);
                _context.Add(post);
                await _context.SaveChangesAsync();
                await SavePostAmenitiesAsync(post.Id, SelectedAmenities);
                return RedirectToAction(nameof(Index));
            }
            var carTypes = _context.CarTypes.Where(c => c.IsDeleted == false);
            ViewData["CarTypeId"] = new SelectList(carTypes, "Id", "Name");
            return View(post);
        }


        // GET: Customer/Posts/Edit/5
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var carTypes = _context.CarTypes.Where(c => c.IsDeleted == false).ToList();
            var amenities = _context.Amenities.Where(a => a.IsDeleted == false).ToList();
            ViewData["CarTypeId"] = new SelectList(carTypes, "Id", "Name");
            ViewData["Amenities"] = new SelectList(amenities, "Id", "Name");
            ViewData["PostAmenities"] = _context.PostAmenities.Where(p => p.PostId == id && p.IsDeleted == false).Select(p => p.Amenity.Name).ToList();
            return View(post);
        }

        private async Task RemovePostAmenities(int postId)
        {
            var postAmenitiesToRemove = _context.PostAmenities.Where(p => p.PostId == postId).ToList();
            _context.PostAmenities.RemoveRange(postAmenitiesToRemove);
            await _context.SaveChangesAsync();
        }

        private bool IsPostAmenitiesChange(int[] SelectedAmenities, int postId)
        {
            var amenities = _context.Amenities.ToList();
            var postAmenities = _context.PostAmenities
                                    .Where(p => p.PostId == postId)
                                    .Select(p => p.Amenity.Name).ToList();
            if (SelectedAmenities.Length > 0)
            {
                // Kiểm tra các checkbox đã chọn và có thay đổi không
                foreach (var item in amenities)
                {
                    bool isChecked = postAmenities != null && postAmenities.Contains(item.Name);
                    bool currentChecked = SelectedAmenities != null && Array.IndexOf(SelectedAmenities, item.Id) != -1;

                    // Nếu trạng thái của checkbox đã thay đổi, đặt giá trị của biến bool là true
                    if (isChecked != currentChecked)
                    {

                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<Post?> GetExistingPost(int id)
                => await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        // POST: Customer/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Image,Description,Seat,RentLocation,HasDriver,Price,Fuel,FuelConsumed,CarTypeId,CompanyId,Id")] Post post, string Gear, IFormFile? Image, List<IFormFile> Images, int[] SelectedAmenities)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var existingPost = await GetExistingPost(id);
            var previosImages = _context.PostImages.Where(p => p.PostId == post.Id).ToList();
            if (Image != null)
            {
                post.Image = await SaveImage(Image);
            }
            else
            {
                post.Image = existingPost!.Image;
            }
            if (Images != null && Images.Count() > 0)
            {
                _context.PostImages.RemoveRange(previosImages);
                await SavePostImage(post, Images);
            }
            else
            {
                post.Images = new List<PostImages>();
                foreach (var item in previosImages)
                {
                    post.Images!.Add(item);
                }
            }
            if (Gear == "Số tự động")
            {
                post.Gear = true;
            }
            else
            {
                post.Gear = false;
            }
            post.CreatedOn = existingPost!.CreatedOn;
            post.CreatedById = existingPost.CreatedById;
            post.UserId = user!.Id;
            post.ModifiedOn = existingPost.ModifiedOn;
            post.ModifiedById = existingPost.ModifiedById;
            post.IsAvailable = true;
            var isChange = IsPostAmenitiesChange(SelectedAmenities, post.Id);
            if (isChange)
            {
                await RemovePostAmenities(post.Id);
                await SavePostAmenitiesAsync(post.Id, SelectedAmenities);
                EditHelper<Post>.SetModifiedIfNecessary(post, true, existingPost, user!.Id);
            }
            else
            {
                bool hasChanges = EditHelper<Post>.HasChanges(post, existingPost); //Hàm kiểm tra
                EditHelper<Post>.SetModifiedIfNecessary(post, hasChanges, existingPost, user!.Id);  //Hàm cập nhật nếu thay đổi
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Name", post.CarTypeId);
            return View(post);
        }

        // GET: Customer/Posts/Delete/5
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.CarType)
                .Include(p => p.Company)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["PostImages"] = _context.PostImages.Where(p => p.PostId == id).Select(p => p.Url).ToList();
            ViewData["PostAmenities"] = _context.PostAmenities.Where(p => p.PostId == id && p.IsDeleted == false).Select(p => p.Amenity.Name).ToList();
            return View(post);
        }

        // POST: Customer/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                post.IsDeleted = true;
                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
        public async Task<IActionResult> GetComment(int id)
        {
            var commment = await _context.Ratings.Where(p => p.PostId == id).ToListAsync();
            return RedirectToAction(nameof(Details));
        }

    }

}
