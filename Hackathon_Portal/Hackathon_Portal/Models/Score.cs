using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon_Portal.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }

        [Required]
        public int HackathonId { get; set; }

        [ForeignKey("HackathonId")]
        public Hackathon? Hackathon { get; set; }

        [Required]
        public int JudgeId { get; set; }

        [ForeignKey("JudgeId")]
        public User? Judge { get; set; }

        [Range(0, 10)]
        public int Innovation { get; set; }

        [Range(0, 10)]
        public int Execution { get; set; }

        [Range(0, 10)]
        public int Presentation { get; set; }

        [NotMapped]
        public int TotalScore => Innovation + Execution + Presentation;

        [MaxLength(500)]
        public string? Feedback { get; set; }

        public DateTime ScoredAt { get; set; } = DateTime.Now;
    }
}
