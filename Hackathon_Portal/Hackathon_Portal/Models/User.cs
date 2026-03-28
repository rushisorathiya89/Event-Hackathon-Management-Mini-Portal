using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Participant"; // Admin, Participant, Judge

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Hackathon>? JudgedHackathons { get; set; }
        public ICollection<Team>? Teams { get; set; }
        public ICollection<Score>? Scores { get; set; }
    }
}
