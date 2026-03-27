namespace Hackathon_Portal.Models;

public class EventRegistration
{
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
}