using Hackathon_Portal.Data;
using Hackathon_Portal.Filters;
using Hackathon_Portal.Models;
using Hackathon_Portal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers
{
    [AuthRequired("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalHackathons = await _db.Hackathons.CountAsync();
            ViewBag.TotalUsers = await _db.Users.CountAsync(u => u.Role == "Participant");
            ViewBag.TotalTeams = await _db.Teams.CountAsync();
            ViewBag.TotalJudges = await _db.Users.CountAsync(u => u.Role == "Judge");
            ViewBag.RecentHackathons = await _db.Hackathons
                .Include(h => h.Judge)
                .OrderByDescending(h => h.CreatedAt)
                .Take(5)
                .ToListAsync();
            return View();
        }

        // GET: Create Hackathon
        public async Task<IActionResult> CreateHackathon()
        {
            var model = new HackathonViewModel
            {
                AvailableJudges = await _db.Users.Where(u => u.Role == "Judge").ToListAsync()
            };
            return View(model);
        }

        // POST: Create Hackathon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHackathon(HackathonViewModel model)
        {
            if (model.RegistrationDeadline >= model.StartDate)
            {
                ModelState.AddModelError("RegistrationDeadline", "Registration deadline must be before the start date.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be on or after the start date.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableJudges = await _db.Users.Where(u => u.Role == "Judge").ToListAsync();
                return View(model);
            }

            var hackathon = new Hackathon
            {
                Title = model.Title,
                Description = model.Description,
                TechStack = model.TechStack,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MaxTeamSize = model.MaxTeamSize,
                RegistrationDeadline = model.RegistrationDeadline,
                JudgeId = model.JudgeId,
                Status = "Draft"
            };

            _db.Hackathons.Add(hackathon);
            await _db.SaveChangesAsync();

            // Notify admin
            await NotificationController.CreateNotification(_db,
                $"Hackathon '{hackathon.Title}' created as Draft.",
                "Info", "fa-calendar-plus", null, "Admin");

            // Notify assigned judge
            if (hackathon.JudgeId != null)
            {
                await NotificationController.CreateNotification(_db,
                    $"You have been assigned as judge for '{hackathon.Title}'.",
                    "Info", "fa-gavel", hackathon.JudgeId,
                    link: "/Judge/MyHackathons");
            }

            return RedirectToAction("Hackathons");
        }

        // List all hackathons
        public async Task<IActionResult> Hackathons()
        {
            var hackathons = await _db.Hackathons
                .Include(h => h.Judge)
                .Include(h => h.Teams)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
            return View(hackathons);
        }

        // GET: Edit Hackathon
        public async Task<IActionResult> EditHackathon(int id)
        {
            var hackathon = await _db.Hackathons.FindAsync(id);
            if (hackathon == null) return RedirectToAction("Hackathons");

            var model = new HackathonViewModel
            {
                Id = hackathon.Id,
                Title = hackathon.Title,
                Description = hackathon.Description,
                TechStack = hackathon.TechStack,
                StartDate = hackathon.StartDate,
                EndDate = hackathon.EndDate,
                RegistrationDeadline = hackathon.RegistrationDeadline,
                MaxTeamSize = hackathon.MaxTeamSize,
                JudgeId = hackathon.JudgeId,
                AvailableJudges = await _db.Users.Where(u => u.Role == "Judge").ToListAsync()
            };

            ViewBag.Status = hackathon.Status;
            return View(model);
        }

        // POST: Edit Hackathon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHackathon(HackathonViewModel model)
        {
            if (model.RegistrationDeadline >= model.StartDate)
            {
                ModelState.AddModelError("RegistrationDeadline", "Registration deadline must be before the start date.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be on or after the start date.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableJudges = await _db.Users.Where(u => u.Role == "Judge").ToListAsync();
                var hack = await _db.Hackathons.AsNoTracking().FirstOrDefaultAsync(h => h.Id == model.Id);
                ViewBag.Status = hack?.Status ?? "Draft";
                return View(model);
            }

            var hackathon = await _db.Hackathons.FindAsync(model.Id);
            if (hackathon == null) return RedirectToAction("Hackathons");

            var oldJudgeId = hackathon.JudgeId;

            hackathon.Title = model.Title;
            hackathon.Description = model.Description;
            hackathon.TechStack = model.TechStack;
            hackathon.StartDate = model.StartDate;
            hackathon.EndDate = model.EndDate;
            hackathon.RegistrationDeadline = model.RegistrationDeadline;
            hackathon.MaxTeamSize = model.MaxTeamSize;
            hackathon.JudgeId = model.JudgeId;

            await _db.SaveChangesAsync();

            // Notify if judge changed
            if (model.JudgeId != null && model.JudgeId != oldJudgeId)
            {
                await NotificationController.CreateNotification(_db,
                    $"You have been assigned as judge for '{hackathon.Title}'.",
                    "Info", "fa-gavel", model.JudgeId,
                    link: "/Judge/MyHackathons");
            }

            TempData["Success"] = $"Hackathon '{hackathon.Title}' updated successfully!";
            return RedirectToAction("Hackathons");
        }

        // Publish hackathon
        [HttpPost]
        public async Task<IActionResult> PublishHackathon(int id)
        {
            var hackathon = await _db.Hackathons.FindAsync(id);
            if (hackathon != null)
            {
                hackathon.Status = "Published";
                await _db.SaveChangesAsync();

                // Notify all participants about new hackathon
                await NotificationController.CreateNotification(_db,
                    $"New hackathon '{hackathon.Title}' is now open! Registration deadline: {hackathon.RegistrationDeadline:MMM dd, yyyy}.",
                    "Success", "fa-rocket", null, "Participant",
                    link: "/Participant/Hackathons");

                // Notify assigned judge
                if (hackathon.JudgeId != null)
                {
                    await NotificationController.CreateNotification(_db,
                        $"Hackathon '{hackathon.Title}' has been published. You are the assigned judge.",
                        "Success", "fa-check-circle", hackathon.JudgeId,
                        link: "/Judge/MyHackathons");
                }
            }
            return RedirectToAction("Hackathons");
        }

        // Cancel (delete) hackathon
        [HttpPost]
        public async Task<IActionResult> CancelHackathon(int id)
        {
            var hackathon = await _db.Hackathons
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Members)
                .Include(h => h.Scores)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hackathon != null)
            {
                var title = hackathon.Title;

                // Remove related notifications
                var relatedNotifications = await _db.Notifications
                    .Where(n => n.Message.Contains(title))
                    .ToListAsync();
                _db.Notifications.RemoveRange(relatedNotifications);

                // Remove scores
                if (hackathon.Scores != null)
                    _db.Scores.RemoveRange(hackathon.Scores);

                // Remove team members and teams
                if (hackathon.Teams != null)
                {
                    // Notify team leaders
                    foreach (var team in hackathon.Teams)
                    {
                        await NotificationController.CreateNotification(_db,
                            $"Hackathon '{title}' has been cancelled. Your team '{team.TeamName}' registration is removed.",
                            "Warning", "fa-triangle-exclamation", team.LeaderId);

                        if (team.Members != null)
                            _db.TeamMembers.RemoveRange(team.Members);
                    }
                    _db.Teams.RemoveRange(hackathon.Teams);
                }

                _db.Hackathons.Remove(hackathon);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Hackathons");
        }

        // View participants for a hackathon
        public async Task<IActionResult> ViewParticipants(int id)
        {
            var hackathon = await _db.Hackathons
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Leader)
                .Include(h => h.Teams!)
                    .ThenInclude(t => t.Members)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hackathon == null) return RedirectToAction("Hackathons");

            return View(hackathon);
        }

        // All registered users
        public async Task<IActionResult> AllUsers()
        {
            var users = await _db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
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

        // GET: Add Judge
        public IActionResult AddJudge()
        {
            return View();
        }

        // POST: Add Judge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddJudge(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required");
                return View();
            }

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View();
            }

            var judge = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Judge"
            };

            _db.Users.Add(judge);
            await _db.SaveChangesAsync();

            // Notify the new judge
            await NotificationController.CreateNotification(_db,
                $"Welcome to HackPortal! Your judge account has been created.",
                "Success", "fa-user-check", judge.Id);

            TempData["Success"] = $"Judge '{fullName}' created successfully!";
            return RedirectToAction("AllUsers");
        }
    }
}
