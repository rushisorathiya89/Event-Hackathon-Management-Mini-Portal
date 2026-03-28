using Hackathon_Portal.Data;
using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDbContext _db;

        public NotificationController(AppDbContext db)
        {
            _db = db;
        }

        private int UserId => HttpContext.Session.GetInt32("UserId") ?? 0;
        private string UserRole => HttpContext.Session.GetString("UserRole") ?? "";

        // GET: api-like endpoint to get notifications for current user
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == UserId || (n.UserId == null && n.TargetRole == UserRole))
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToListAsync();

            var unreadCount = notifications.Count(n => !n.IsRead);

            return Json(new { unreadCount, notifications });
        }

        // POST: Mark single notification as read
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _db.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _db.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        // POST: Mark all notifications as read for current user
        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var notifications = await _db.Notifications
                .Where(n => (n.UserId == UserId || (n.UserId == null && n.TargetRole == UserRole)) && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        // Full notifications page
        public async Task<IActionResult> All()
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == UserId || (n.UserId == null && n.TargetRole == UserRole))
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();

            // Determine which layout to use
            ViewBag.Layout = UserRole switch
            {
                "Admin" => "~/Views/_AdminLayout.cshtml",
                "Judge" => "~/Views/_JudgeLayout.cshtml",
                _ => "~/Views/_ParticipantLayout.cshtml"
            };

            return View(notifications);
        }

        // Helper: Create notification (called from other controllers)
        public static async Task CreateNotification(AppDbContext db, string message, string type = "Info",
            string icon = "fa-bell", int? userId = null, string? targetRole = null, string? link = null)
        {
            db.Notifications.Add(new Notification
            {
                UserId = userId,
                TargetRole = targetRole,
                Message = message,
                Type = type,
                Icon = icon,
                Link = link
            });
            await db.SaveChangesAsync();
        }
    }
}
