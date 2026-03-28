using System.Diagnostics;
using Hackathon_Portal.Data;
using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var publishedHackathons = await _db.Hackathons
                .Where(h => h.Status == "Published")
                .OrderByDescending(h => h.CreatedAt)
                .Take(6)
                .ToListAsync();

            ViewBag.TotalHackathons = await _db.Hackathons.CountAsync(h => h.Status == "Published");
            ViewBag.TotalUsers = await _db.Users.CountAsync(u => u.Role == "Participant");
            ViewBag.TotalTeams = await _db.Teams.CountAsync();

            return View(publishedHackathons);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
