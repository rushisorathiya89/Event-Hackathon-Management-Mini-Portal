namespace Hackathon_Portal.Models;

public class HomePageViewModel
{
    public int TotalEvents { get; set; }

    public int TotalTeams { get; set; }

    public int TotalJudges { get; set; }

    public ICollection<LeaderboardItemViewModel> Leaderboard { get; set; } = new List<LeaderboardItemViewModel>();
}

public class LeaderboardItemViewModel
{
    public int Rank { get; set; }

    public string TeamName { get; set; } = string.Empty;

    public int TotalPoints { get; set; }
}