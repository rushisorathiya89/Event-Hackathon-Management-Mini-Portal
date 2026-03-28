using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models.ViewModels
{
    public class TeamRegistrationViewModel
    {
        public int HackathonId { get; set; }
        public string? HackathonTitle { get; set; }

        [Required(ErrorMessage = "Team name is required")]
        [MaxLength(100)]
        public string TeamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project title is required")]
        [MaxLength(200)]
        public string ProjectTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string ProjectDescription { get; set; } = string.Empty;

        public List<MemberInput> Members { get; set; } = new List<MemberInput>();
    }

    public class MemberInput
    {
        [Required(ErrorMessage = "Member name is required")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;
    }
}
