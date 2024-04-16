using Domain.Common;

namespace Domain.Entities;

public class SubscriptionHeading: BaseAuditableEntity
{
    internal SubscriptionHeading(string title, DateTime created)
    {
        Title = title;
        Created = created;
    }

    //Properties
    public string Title { get; set; } = null!;
  
    //Foreign Key
    public int SubscriptionId { get; private set; }
    public Subscription Subscription { get; private set; } = null!;

    /// <summary>
    /// Subscription Points
    /// </summary>
    private readonly List<SubscriptionPoint> _subscriptionPoints = new();
    public IReadOnlyCollection<SubscriptionPoint> SubscriptionPoints => _subscriptionPoints.AsReadOnly();


    //Methods
    public void setSubscriptionObjectRefrence(Subscription subscription)
    {
        Subscription = subscription;
    }
}
