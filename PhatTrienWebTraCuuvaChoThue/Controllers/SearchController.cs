using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhatTrienWebTraCuuvaChoThue.Models;

namespace PhatTrienWebTraCuuvaChoThue.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Search
        public async Task<IActionResult> Index(string? keyword, int? categoryId, int? locationId, decimal? minPrice, decimal? maxPrice, double? minArea)
        {
            var query = _context.Listings
                .Include(l => l.Category)
                .Include(l => l.Location)
                .Include(l => l.Owner)
                .Include(l => l.ListingImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                string kw = keyword.ToLower();
                query = query.Where(l => l.Title.ToLower().Contains(kw) ||
                                         (l.Description != null && l.Description.ToLower().Contains(kw)));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(l => l.CategoryId == categoryId.Value);
            }

            if (locationId.HasValue)
            {
                query = query.Where(l => l.LocationId == locationId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(l => l.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(l => l.Price <= maxPrice.Value);
            }

            if (minArea.HasValue)
            {
                query = query.Where(l => l.Area >= minArea.Value);
            }

            ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Name", categoryId);
            ViewData["Locations"] = new SelectList(_context.Locations, "Id", "Province", locationId);

            return View(await query.OrderByDescending(l => l.CreatedAt).ToListAsync());
        }

    }
}
