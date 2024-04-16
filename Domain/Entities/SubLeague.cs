
using Domain.Common;

namespace Domain.Entities;

public class SubLeague: BaseAuditableEntity
{
    internal SubLeague(string? icon, string name, int minRange, int maxRange, DateTime created, bool deleted)
    {
        Icon = icon;
        Name = name;
        MinRange = minRange;
        MaxRange = maxRange;
        Deleted = deleted;
        Created = created;
    }

    //Properties
    public string? Icon { get; private set; }
    public string Name { get; private set; } = null!;
    public int MinRange { get; private set; }
    public int MaxRange { get; private set; }

    private bool _deleted;
    public bool Deleted
    {
        get => _deleted;
        set
        {
            if (value == true && _deleted == false)
            {
                //Trigger Domain Event if any 
            }

            _deleted = value;
        }
    }

    //Foreign Key
    public int LeagueId { get; private set; }
    public League League { get; private set; } = null!;

    /// <summary>
    /// League Reward
    /// </summary>
    private readonly List<LeagueReward> _leagueRewards = new();
    public IReadOnlyCollection<LeagueReward> LeagueRewards => _leagueRewards.AsReadOnly();

    /// <summary>
    /// User Setting
    /// </summary>
    private readonly List<UserSetting> _userSettings = new();
    public IReadOnlyCollection<UserSetting> UserSettings => _userSettings.AsReadOnly();


    //Methods
    public void setLeagueObjectRefrence(League league)
    {
        League = league;
    }

    public void UpdateSubLeague(string? icon, string name, int minrange, int maxrange)
    {
        Icon = icon;
        Name = name;
        MinRange = minrange;
        MaxRange = maxrange;
    }
}

