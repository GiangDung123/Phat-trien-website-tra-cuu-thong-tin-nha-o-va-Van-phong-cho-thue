using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhatTrienWebTraCuuvaChoThue.Models;

namespace PhatTrienWebTraCuuvaChoThue.Controllers
{
    public class ListingsController : Controller
    {
        private readonly AppDbContext _context;

        public ListingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            var listings = _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.Owner);

            return View(await listings.ToListAsync());
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.Owner)
                .Include(l => l.ListingImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            return listing == null ? NotFound() : View(listing);
        }
        // GET: Listings/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Province");
            return View();
        }

        // POST: Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,Area,CategoryId,LocationId")] Listing listing, List<IFormFile>? uploadedImages)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Lỗi ở {key}: {error.ErrorMessage}");
                    }
                }

                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", listing.CategoryId);
                ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Province", listing.LocationId);
                return View(listing);
            }

            listing.CreatedAt = DateTime.Now;
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");
            listing.OwnerId = userId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(listing);
                await _context.SaveChangesAsync(); 

                if (uploadedImages != null && uploadedImages.Any())
                {
                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(folder);

                    foreach (var file in uploadedImages)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(folder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var image = new ListingImage
                            {
                                ListingId = listing.Id,
                                ImageUrl = "/uploads/" + fileName
                            };
                            _context.ListingImages.Add(image);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", listing.CategoryId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Province", listing.LocationId);
            return View(listing);
        }



        // GET: Listings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRole = HttpContext.Session.GetString("Role");

            if (currentRole != "Admin" && listing.OwnerId != currentUserId)
            {
                return Forbid();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", listing.CategoryId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Province", listing.LocationId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "FullName", listing.OwnerId);
            return View(listing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,Area,CreatedAt,CategoryId,LocationId,OwnerId")] Listing listing, List<IFormFile>? uploadedImages)
        {
            if (id != listing.Id) return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRole = HttpContext.Session.GetString("Role");

            if (currentRole != "Admin" && listing.OwnerId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listing);
                    await _context.SaveChangesAsync();

                    if (uploadedImages != null && uploadedImages.Any())
                    {
                        string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        Directory.CreateDirectory(folder);

                        foreach (var file in uploadedImages)
                        {
                            if (file.Length > 0)
                            {
                                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                                string filePath = Path.Combine(folder, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var image = new ListingImage
                                {
                                    ListingId = listing.Id,
                                    ImageUrl = "/uploads/" + fileName
                                };
                                _context.ListingImages.Add(image);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingExists(listing.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", listing.CategoryId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Province", listing.LocationId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "FullName", listing.OwnerId);
            return View(listing);
        }


        // GET: Listings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var listing = await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            return listing == null ? NotFound() : View(listing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.ListingImages)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing == null)
                return NotFound();

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRole = HttpContext.Session.GetString("Role");

            if (currentRole != "Admin" && listing.OwnerId != currentUserId)
            {
                return Forbid();
            }

            foreach (var img in listing.ListingImages)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.ListingImages.RemoveRange(listing.ListingImages);
            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private bool ListingExists(int id)
        {
            return _context.Listings.Any(e => e.Id == id);
        }
    }
}
