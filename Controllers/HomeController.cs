using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentEase.Data;

namespace RentEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProperties = await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .Where(p => p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var stats = new
            {
                TotalListings = await _context.Properties.CountAsync(p => p.IsAvailable),
                Cities = await _context.Properties.Select(p => p.City).Distinct().CountAsync(),
                Users = await _context.Users.CountAsync()
            };

            ViewBag.FeaturedProperties = featuredProperties;
            ViewBag.Stats = stats;

            return View();
        }

        public IActionResult Privacy() => View();
        public IActionResult About() => View();
    }
}
