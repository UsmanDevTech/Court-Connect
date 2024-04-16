using Domain.Abstraction;
using Domain.Common;

namespace Domain.Entities;

public class League : BaseAuditableEntity, ISoftDelete
{
    private League(string? icon, string name, int minRange, int maxRange, string createdBy, DateTime created, bool deleted)
    {
        Icon = icon;
        Name = name;
        MinRange = minRange;
        MaxRange = maxRange;
        Deleted = deleted;
        CreatedBy = createdBy;
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
    /// <summary>
    /// Sub Leagues
    /// </summary>
    private readonly List<SubLeague> _subLeagues = new();
    public IReadOnlyCollection<SubLeague> SubLeagues => _subLeagues.AsReadOnly();


    //Factory Method
    public static League Create(string? icon, string name, int minRange, int maxRange, string createdBy, DateTime created)
    {
        return new League(icon, name, minRange, maxRange, createdBy, created, false);
    }

    public void AddSubLeague(string? icon, string name,
        int minrange, int maxrange,DateTime created,bool deleted)
    {
        var subleague = new SubLeague(icon,name,minrange,maxrange,created,deleted);
        subleague.setLeagueObjectRefrence(this);

        _subLeagues.Add(subleague);
    }

    public void UpdateLeague(string? icon, string name,int minrange, int maxrange)
    {
        Icon = icon;
        Name = name;
        MinRange = minrange;
        MaxRange = maxrange;
    }

}