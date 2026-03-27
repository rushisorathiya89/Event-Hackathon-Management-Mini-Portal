using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models;

public class Team
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}