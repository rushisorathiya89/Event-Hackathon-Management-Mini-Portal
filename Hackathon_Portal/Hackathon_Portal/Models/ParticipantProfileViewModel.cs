namespace Hackathon_Portal.Models;

public class ParticipantProfileViewModel
{
    public string CurrentTeamName { get; set; } = "Not assigned";

    public string CurrentEventTitle { get; set; } = "No active event";

    public double AverageScore { get; set; }

    public int ScoredSubmissionsCount { get; set; }

    public ICollection<ParticipantHistoryItemViewModel> History { get; set; } = new List<ParticipantHistoryItemViewModel>();
}

public class ParticipantHistoryItemViewModel
{
    public string PastEventTitle { get; set; } = string.Empty;

    public string TeamName { get; set; } = string.Empty;

    public int FinalRank { get; set; }
}