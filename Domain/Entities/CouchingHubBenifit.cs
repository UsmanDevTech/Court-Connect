using Domain.Common;

namespace Domain.Entities;

public class CouchingHubBenifit : BaseAuditableEntity
{
    internal CouchingHubBenifit(string? icon, string detail, DateTime created)
    {
        Icon = icon;
        Detail = detail;
        Created = created;
    }

    //Properties
    public string? Icon { get; set; }
    public string Detail { get; set; } = null!;

    //Foreign Key
    public int CouchingHubId { get; private set; }
    public CouchingHub CouchingHub { get; private set; } = null!;


    //Methods
    public void setCouchingHubObjectRefrence(CouchingHub couchingHub)
    {
        CouchingHub = couchingHub;
    }
}