using Microsoft.AspNetCore.Identity;

namespace Hackathon_Portal.Models;

public class AdminDashboardViewModel
{
    public int TotalTeams { get; set; }

    public int TotalJudges { get; set; }

    public int TotalEvents { get; set; }

    public IEnumerable<Event> ActiveEvents { get; set; } = new List<Event>();

    public IEnumerable<IdentityUser> Judges { get; set; } = new List<IdentityUser>();

    public IEnumerable<TeamRanking> TopTeams { get; set; } = new List<TeamRanking>();
}
