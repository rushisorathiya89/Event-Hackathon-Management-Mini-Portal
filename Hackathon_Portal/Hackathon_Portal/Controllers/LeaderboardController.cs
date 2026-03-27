using Hackathon_Portal.Data;
using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers;

public class LeaderboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaderboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? eventId)
    {
        var allEvents = await _context.Events.ToListAsync();

        Event? selectedEvent = null;

        if (eventId.HasValue)
        {
            selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId.Value);
        }

        if (selectedEvent == null && allEvents.Any())
        {
            selectedEvent = allEvents.OrderByDescending(e => e.StartDate).FirstOrDefault();
        }

        var rankings = new List<TeamRanking>();

        if (selectedEvent != null)
        {
            var scores = await _context.Scores
                .Where(s => s.EventId == selectedEvent.Id)
                .Include(s => s.Team)
                .ToListAsync();

            var rankingData = scores
                .GroupBy(s => new { s.TeamId, s.Team.Name })
                .Select(g => new
                {
                    TeamId = g.Key.TeamId,
                    TeamName = g.Key.Name,
                    AverageTechnical = g.Average(s => s.TechnicalScore),
                    AverageInnovation = g.Average(s => s.InnovationScore)
                })
                .OrderByDescending(r => (r.AverageTechnical + r.AverageInnovation) / 2)
                .ToList();

            int rank = 1;
            foreach (var data in rankingData)
            {
                rankings.Add(new TeamRanking
                {
                    Rank = rank++,
                    TeamId = data.TeamId,
                    TeamName = data.TeamName,
                    AverageTechnical = Math.Round(data.AverageTechnical, 1),
                    AverageInnovation = Math.Round(data.AverageInnovation, 1),
                    FinalScore = Math.Round((data.AverageTechnical + data.AverageInnovation) / 2, 1)
                });
            }
        }

        var model = new LeaderboardViewModel
        {
            PastEvents = allEvents,
            SelectedEvent = selectedEvent,
            Rankings = rankings
        };

        return View(model);
    }
}
