using Domain.Common;

namespace Domain.Entities;

public class SubscriptionPoint: BaseAuditableEntity
{
    internal SubscriptionPoint(string? icon, string detail, DateTime created)
    {
        Icon = icon;
        Detail = detail;
        Created = created;
    }

    //Properties
    public string? Icon { get; set; }
    public string Detail { get; set; } = null!;

    //Foreign Key
    public int SubscriptionHeadingId { get; private set; }
    public SubscriptionHeading SubscriptionHeading { get; private set; } = null!;


    //Methods
    public void setSubscriptionHeadingObjectRefrence(SubscriptionHeading subscriptionHeading)
    {
        SubscriptionHeading = subscriptionHeading;
    }
}


