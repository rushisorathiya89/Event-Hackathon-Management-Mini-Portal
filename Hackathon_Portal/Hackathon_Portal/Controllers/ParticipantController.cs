using Hackathon_Portal.Data;
using Hackathon_Portal.Filters;
using Hackathon_Portal.Models;
using Hackathon_Portal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers
{
    [AuthRequired("Participant")]
    public class ParticipantController : Controller
    {
        private readonly AppDbContext _db;

        public ParticipantController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalPublished = await _db.Hackathons.CountAsync(h => h.Status == "Published");
            ViewBag.MyTeams = await _db.Teams.CountAsync(t => t.LeaderId == UserId);

            // Unread notification count
            ViewBag.UnreadNotifications = await _db.Notifications
                .CountAsync(n => (n.UserId == UserId || (n.UserId == null && n.TargetRole == "Participant")) && !n.IsRead);

            return View();
        }

        // Browse all published hackathons
        public async Task<IActionResult> Hackathons()
        {
            var hackathons = await _db.Hackathons
                .Where(h => h.Status == "Published")
                .Include(h => h.Teams)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            // Check which ones user already applied to
            var appliedIds = await _db.Teams
                .Where(t => t.LeaderId == UserId)
                .Select(t => t.HackathonId)
                .ToListAsync();

            ViewBag.AppliedIds = appliedIds;
            ViewBag.Now = DateTime.Now;
            return View(hackathons);
        }

        // GET: Apply to hackathon
        public async Task<IActionResult> Apply(int id)
        {
            var hackathon = await _db.Hackathons.FindAsync(id);
            if (hackathon == null || hackathon.Status != "Published")
                return RedirectToAction("Hackathons");

            // Check deadline
            if (DateTime.Now > hackathon.RegistrationDeadline)
            {
                TempData["Error"] = "Registration deadline has passed for this hackathon.";
                return RedirectToAction("Hackathons");
            }

            // Check if already applied
            var existing = await _db.Teams.AnyAsync(t => t.HackathonId == id && t.LeaderId == UserId);
            if (existing)
            {
                TempData["Error"] = "You have already applied to this hackathon.";
                return RedirectToAction("Hackathons");
            }

            var model = new TeamRegistrationViewModel
            {
                HackathonId = id,
                HackathonTitle = hackathon.Title,
                Members = new List<MemberInput> { new MemberInput() }
            };

            ViewBag.MaxTeamSize = hackathon.MaxTeamSize;
            ViewBag.Deadline = hackathon.RegistrationDeadline;
            return View(model);
        }

        // POST: Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(TeamRegistrationViewModel model)
        {
            var hackathon = await _db.Hackathons.FindAsync(model.HackathonId);
            if (hackathon == null) return RedirectToAction("Hackathons");

            // Enforce deadline on POST too
            if (DateTime.Now > hackathon.RegistrationDeadline)
            {
                TempData["Error"] = "Registration deadline has passed for this hackathon.";
                return RedirectToAction("Hackathons");
            }

            // Remove empty members
            model.Members = model.Members?.Where(m => !string.IsNullOrWhiteSpace(m.Name)).ToList()
                ?? new List<MemberInput>();

            var userName = HttpContext.Session.GetString("UserName") ?? "A participant";

            var team = new Team
            {
                TeamName = model.TeamName,
                HackathonId = model.HackathonId,
                LeaderId = UserId,
                ProjectTitle = model.ProjectTitle,
                ProjectDescription = model.ProjectDescription
            };

            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            // Add members
            foreach (var member in model.Members)
            {
                _db.TeamMembers.Add(new TeamMember
                {
                    TeamId = team.Id,
                    MemberName = member.Name,
                    MemberEmail = member.Email
                });
            }
            await _db.SaveChangesAsync();

            // Notify admin about new registration
            await NotificationController.CreateNotification(_db,
                $"Team '{model.TeamName}' registered for '{hackathon.Title}' by {userName}.",
                "Info", "fa-user-group", null, "Admin",
                link: $"/Admin/ViewParticipants/{hackathon.Id}");

            // Notify the participant
            await NotificationController.CreateNotification(_db,
                $"You successfully registered for '{hackathon.Title}' with team '{model.TeamName}'!",
                "Success", "fa-check-circle", UserId,
                link: "/Participant/MyHackathons");

            TempData["Success"] = "Successfully applied to hackathon!";
            return RedirectToAction("MyHackathons");
        }

        // My hackathons (applied)
        public async Task<IActionResult> MyHackathons()
        {
            var teams = await _db.Teams
                .Where(t => t.LeaderId == UserId)
                .Include(t => t.Hackathon)
                .Include(t => t.Members)
                .Include(t => t.Scores)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(teams);
        }

        // Results for a hackathon
        public async Task<IActionResult> Results(int id)
        {
            var hackathon = await _db.Hackathons
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Scores)
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Leader)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hackathon == null) return RedirectToAction("Hackathons");

            return View(hackathon);
        }

        // Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _db.Users.FindAsync(UserId);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.TeamCount = await _db.Teams.CountAsync(t => t.LeaderId == UserId);
            return View(user);
        }
    }
}
