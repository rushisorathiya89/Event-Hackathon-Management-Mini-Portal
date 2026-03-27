using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hackathon_Portal.Data;
using Hackathon_Portal.Models;

namespace Hackathon_Portal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var judges = await _userManager.GetUsersInRoleAsync("Judge");
        var events = await _context.Events.ToListAsync();
        var teams = await _context.Teams.ToListAsync();

        // Calculate top teams by average score
        var topTeams = await _context.Scores
            .GroupBy(s => new { s.TeamId, s.Team.Name })
            .Select(g => new TeamRanking
            {
                TeamId = g.Key.TeamId,
                TeamName = g.Key.Name,
                AverageTechnical = g.Average(s => s.TechnicalScore),
                AverageInnovation = g.Average(s => s.InnovationScore),
                FinalScore = (g.Average(s => s.TechnicalScore) + g.Average(s => s.InnovationScore)) / 2
            })
            .OrderByDescending(r => r.FinalScore)
            .Take(5)
            .ToListAsync();

        int rank = 1;
        foreach (var team in topTeams)
        {
            team.Rank = rank++;
        }

        var model = new AdminDashboardViewModel
        {
            TotalTeams = teams.Count,
            TotalJudges = judges.Count,
            TotalEvents = events.Count,
            ActiveEvents = events,
            Judges = judges,
            TopTeams = topTeams
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleRegistration(int eventId)
    {
        var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        if (eventItem == null)
        {
            return NotFound();
        }

        eventItem.IsRegistrationOpen = !eventItem.IsRegistrationOpen;
        _context.Events.Update(eventItem);
        await _context.SaveChangesAsync();

        return Ok(new { isOpen = eventItem.IsRegistrationOpen });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddJudge(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["Error"] = "Email is required.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["Error"] = $"User with email '{email}' not found.";
            return RedirectToAction(nameof(Index));
        }

        var isInRole = await _userManager.IsInRoleAsync(user, "Judge");
        if (isInRole)
        {
            TempData["Error"] = $"User '{email}' is already a Judge.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.AddToRoleAsync(user, "Judge");
        if (result.Succeeded)
        {
            TempData["Success"] = $"User '{email}' has been added as a Judge.";
        }
        else
        {
            TempData["Error"] = "Failed to add user as Judge.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeJudge(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.RemoveFromRoleAsync(user, "Judge");
        if (result.Succeeded)
        {
            TempData["Success"] = $"Judge privileges revoked for {user.Email}.";
        }
        else
        {
            TempData["Error"] = "Failed to revoke Judge privileges.";
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Events()
    {
        return View();
    }

    public IActionResult Judges()
    {
        return View();
    }

    public IActionResult TeamsRegistration()
    {
        return View();
    }

    public IActionResult Leaderboard()
    {
        return View();
    }
}