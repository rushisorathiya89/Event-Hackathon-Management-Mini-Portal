using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon_Portal.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string TeamName { get; set; } = string.Empty;

        [Required]
        public int HackathonId { get; set; }

        [ForeignKey("HackathonId")]
        public Hackathon? Hackathon { get; set; }

        [Required]
        public int LeaderId { get; set; }

        [ForeignKey("LeaderId")]
        public User? Leader { get; set; }

        [MaxLength(200)]
        public string ProjectTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string ProjectDescription { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<TeamMember>? Members { get; set; }
        public ICollection<Score>? Scores { get; set; }
    }
}
