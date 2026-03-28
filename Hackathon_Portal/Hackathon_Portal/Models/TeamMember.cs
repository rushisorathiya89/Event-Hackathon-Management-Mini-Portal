using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon_Portal.Models
{
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }

        [Required, MaxLength(100)]
        public string MemberName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string MemberEmail { get; set; } = string.Empty;
    }
}
