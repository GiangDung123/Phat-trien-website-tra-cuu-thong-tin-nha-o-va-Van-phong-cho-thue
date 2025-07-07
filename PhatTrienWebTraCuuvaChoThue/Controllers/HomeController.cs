using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhatTrienWebTraCuuvaChoThue.Models;

namespace PhatTrienWebTraCuuvaChoThue.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listings = await _context.Listings
                .Include(l => l.Location)
                .Include(l => l.ListingImages)
                .OrderByDescending(l => l.CreatedAt)
                .Take(9) 
                .ToListAsync();

            return View(listings);
        }
    }
}
