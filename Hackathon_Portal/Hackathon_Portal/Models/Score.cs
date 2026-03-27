namespace Hackathon_Portal.Models;

public class Score
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int JudgeId { get; set; }

    public double TechnicalScore { get; set; }

    public double InnovationScore { get; set; }

    public DateTime SubmittedAt { get; set; }
}
