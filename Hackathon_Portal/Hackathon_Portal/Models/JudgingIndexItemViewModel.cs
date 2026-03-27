namespace Hackathon_Portal.Models;

public class JudgingIndexItemViewModel
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = string.Empty;

    public string ProjectTitle { get; set; } = string.Empty;

    public string GitHubUrl { get; set; } = string.Empty;

    // True when the current judge has already submitted a score for this team.
    public bool HasScore { get; set; }
}