using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models.ViewModels
{
    public class HackathonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TechStack { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

        [Range(2, 10, ErrorMessage = "Team size must be between 2 and 10")]
        public int MaxTeamSize { get; set; } = 4;

        [Required(ErrorMessage = "Registration deadline is required")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDeadline { get; set; } = DateTime.Now.AddDays(6);

        public int? JudgeId { get; set; }

        // For the dropdown
        public List<User>? AvailableJudges { get; set; }
    }
}
