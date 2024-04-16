using Domain.Abstraction;
using Domain.Common;

namespace Domain.Entities;
public class CouchingHubCategory : BaseAuditableEntity, ISoftDelete
{
    private CouchingHubCategory(string? icon, string name, string createdBy, DateTime created)
    {
        Icon = icon;
        Name = name;
        CreatedBy = createdBy;
        Created = created;
    }

    public CouchingHubCategory(string icon, string name)
    {
        Icon = icon;
        Name = name;
    }

    //Properties
    public string? Icon { get; private set; }
    public string Name { get; private set; } = null!;
    
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
    /// Couching Hub
    /// </summary>
    private readonly List<CouchingHub> _couchingHubs = new();
    public IReadOnlyCollection<CouchingHub> CouchingHubs => _couchingHubs.AsReadOnly();

    public void SetNameandIcon(string? icon, string name)
    {
        Icon = icon;
        Name = name;
    }

    //Factory Method
    public static CouchingHubCategory Create(string? icon, string name, string createdBy, DateTime created)
    {
        return new CouchingHubCategory(icon, name, createdBy, created);
    }
}

