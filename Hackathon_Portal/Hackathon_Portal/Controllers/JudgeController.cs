using Hackathon_Portal.Data;
using Hackathon_Portal.Filters;
using Hackathon_Portal.Models;
using Hackathon_Portal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers
{
    [AuthRequired("Judge")]
    public class JudgeController : Controller
    {
        private readonly AppDbContext _db;

        public JudgeController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.AssignedCount = await _db.Hackathons
                .CountAsync(h => h.JudgeId == UserId && h.Status == "Published");
            ViewBag.TotalScored = await _db.Scores.CountAsync(s => s.JudgeId == UserId);
            return View();
        }

        // My assigned hackathons only
        public async Task<IActionResult> MyHackathons()
        {
            var hackathons = await _db.Hackathons
                .Where(h => h.JudgeId == UserId && h.Status == "Published")
                .Include(h => h.Teams)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return View(hackathons);
        }

        // Score teams for a hackathon
        public async Task<IActionResult> ScoreTeams(int id)
        {
            var hackathon = await _db.Hackathons
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Members)
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Scores)
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Leader)
                .FirstOrDefaultAsync(h => h.Id == id && h.JudgeId == UserId);

            if (hackathon == null) return RedirectToAction("MyHackathons");

            return View(hackathon);
        }

        // POST: Submit score for a team
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitScore(ScoreViewModel model)
        {
            // Check if already scored
            var existing = await _db.Scores
                .FirstOrDefaultAsync(s => s.TeamId == model.TeamId && s.JudgeId == UserId);

            if (existing != null)
            {
                existing.Innovation = model.Innovation;
                existing.Execution = model.Execution;
                existing.Presentation = model.Presentation;
                existing.Feedback = model.Feedback;
                existing.ScoredAt = DateTime.Now;
            }
            else
            {
                var score = new Score
                {
                    TeamId = model.TeamId,
                    HackathonId = model.HackathonId,
                    JudgeId = UserId,
                    Innovation = model.Innovation,
                    Execution = model.Execution,
                    Presentation = model.Presentation,
                    Feedback = model.Feedback
                };
                _db.Scores.Add(score);
            }

            await _db.SaveChangesAsync();

            // Notify the team leader that their team was scored
            var team = await _db.Teams.Include(t => t.Hackathon).FirstOrDefaultAsync(t => t.Id == model.TeamId);
            if (team != null)
            {
                var judgeName = HttpContext.Session.GetString("UserName") ?? "A judge";
                var totalScore = model.Innovation + model.Execution + model.Presentation;

                await NotificationController.CreateNotification(_db,
                    $"Your team '{team.TeamName}' in '{team.Hackathon?.Title}' has been scored: {totalScore}/30.",
                    "Success", "fa-star", team.LeaderId,
                    link: $"/Participant/Results/{model.HackathonId}");

                // Notify admin
                await NotificationController.CreateNotification(_db,
                    $"Judge {judgeName} scored team '{team.TeamName}' in '{team.Hackathon?.Title}': {totalScore}/30.",
                    "Info", "fa-clipboard-check", null, "Admin",
                    link: $"/Admin/Results/{model.HackathonId}");
            }

            TempData["Success"] = "Score submitted successfully!";
            return RedirectToAction("ScoreTeams", new { id = model.HackathonId });
        }

        // Results
        public async Task<IActionResult> Results(int id)
        {
            var hackathon = await _db.Hackathons
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Scores)
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Leader)
                .FirstOrDefaultAsync(h => h.Id == id && h.JudgeId == UserId);

            if (hackathon == null) return RedirectToAction("MyHackathons");

            return View(hackathon);
        }
    }
}
