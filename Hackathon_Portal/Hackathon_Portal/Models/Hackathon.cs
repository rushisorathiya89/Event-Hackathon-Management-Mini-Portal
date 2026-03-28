using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon_Portal.Models
{
    public class Hackathon
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TechStack { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int MaxTeamSize { get; set; } = 4;

        // Registration deadline - after this date, participants cannot apply
        [Required]
        public DateTime RegistrationDeadline { get; set; }

        // Judge assignment
        public int? JudgeId { get; set; }

        [ForeignKey("JudgeId")]
        public User? Judge { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Published, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Team>? Teams { get; set; }
        public ICollection<Score>? Scores { get; set; }
    }
}
