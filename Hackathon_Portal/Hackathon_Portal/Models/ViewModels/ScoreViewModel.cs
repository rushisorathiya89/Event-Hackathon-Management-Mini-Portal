using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models.ViewModels
{
    public class ScoreViewModel
    {
        public int TeamId { get; set; }
        public int HackathonId { get; set; }
        public string? TeamName { get; set; }
        public string? ProjectTitle { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Score must be 0-10")]
        public int Innovation { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Score must be 0-10")]
        public int Execution { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Score must be 0-10")]
        public int Presentation { get; set; }

        [MaxLength(500)]
        public string? Feedback { get; set; }
    }
}
