using System.ComponentModel.DataAnnotations;

namespace Hackathon_Portal.Models;

public class Event
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1500)]
    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsRegistrationOpen { get; set; }

    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}