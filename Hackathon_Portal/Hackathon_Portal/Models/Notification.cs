using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon_Portal.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        // Who receives the notification (null = all users of a role)
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Target role for broadcast notifications (e.g., "Participant", "Judge")
        [MaxLength(20)]
        public string? TargetRole { get; set; }

        [Required, MaxLength(300)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Alert

        [MaxLength(20)]
        public string Icon { get; set; } = "fa-bell"; // FontAwesome icon class

        // Optional link to navigate to
        [MaxLength(300)]
        public string? Link { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
