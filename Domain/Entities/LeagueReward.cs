using Domain.Common;

namespace Domain.Entities;

public class LeagueReward: BaseAuditableEntity
{
    internal LeagueReward(string? icon, string detail, DateTime created)
    {
        Icon = icon;
        Detail = detail;
        Created = created;
    }

    //Properties
    public string? Icon { get; set; }
    public string Detail { get; set; } = null!;

    //Foreign Key
    public int SubLeagueId { get; private set; }
    public SubLeague SubLeague { get; private set; } = null!;

 
    //Methods
    public void setSubLeagueObjectRefrence(SubLeague subLeague)
    {
        SubLeague = subLeague;
    }
}
