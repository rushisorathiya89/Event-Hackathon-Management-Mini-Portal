namespace Hackathon_Portal.Models;

public class LeaderboardViewModel
{
    public IEnumerable<Event> PastEvents { get; set; } = new List<Event>();

    public Event? SelectedEvent { get; set; }

    public IEnumerable<TeamRanking> Rankings { get; set; } = new List<TeamRanking>();
}

public class TeamRanking
{
    public int Rank { get; set; }

    public int TeamId { get; set; }

    public string TeamName { get; set; } = string.Empty;

    public double AverageTechnical { get; set; }

    public double AverageInnovation { get; set; }

    public double FinalScore { get; set; }
}
