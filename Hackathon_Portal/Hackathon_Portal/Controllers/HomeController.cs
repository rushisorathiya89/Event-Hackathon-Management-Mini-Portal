using System.Diagnostics;
using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon_Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                TotalEvents = 15,
                TotalTeams = 1200,
                TotalJudges = 50,
                Leaderboard = new List<LeaderboardItemViewModel>
                {
                    new() { Rank = 1, TeamName = "Null Pointers", TotalPoints = 12450 },
                    new() { Rank = 2, TeamName = "Binary Bandits", TotalPoints = 11920 },
                    new() { Rank = 3, TeamName = "Cyber Sentinels", TotalPoints = 10840 },
                    new() { Rank = 4, TeamName = "Algorithm Alchemists", TotalPoints = 9710 },
                    new() { Rank = 5, TeamName = "Cloud Walkers", TotalPoints = 9305 }
                }
            };

            return View(model);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
