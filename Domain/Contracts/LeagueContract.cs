
namespace Domain.Contracts;

public class LeagueContract
{
    public int id { get; set; } 
    public string? icon { get; set; }
    public string name { get; set; } = null!;
    public int minRange { get; set; }
    public int maxRange { get; set; }
    public string created { get; set; } = null!;
}

public class AdminLeagueContract: LeagueContract
{
    public bool deleted { get; set; }
}

public sealed class LeagueRankingContract: LeagueContract
{
    public List<LeagueRankingUserContract> leagueUsers { get; set; }
}
public class SubLeagueContract
{
    public int id { get; set; }
    public string? icon { get; set; }
    public string name { get; set; } = null!;
    public int minRange { get; set; }
    public int maxRange { get; set; }
    public string created { get; set; } = null!;
    public List<GenericIconInfoContract>? points { get; set; }
}

public sealed class LeagueSubLeagueDetailContract : LeagueContract
{
    public List<SubLeagueContract> subLeagues { get; set; } = new();
}

public sealed class SubleagueStatusContract : SubLeagueContract
{
    public bool status { get; set; } = new();
}