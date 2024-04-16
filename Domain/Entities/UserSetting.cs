using Domain.Common;

namespace Domain.Entities;

public sealed class UserSetting : BaseAuditableEntity
{
    private UserSetting(string userId, int subLeagueId, int? subscriptionId)
    {
        UserId = userId;
        SubLeagueId = subLeagueId;
        SubscriptionId = subscriptionId;
    }

    public string UserId { get; set; } = null!;

    /// <summary>
    /// foreign reference
    /// </summary>
    public int SubLeagueId { get; set; }
    public SubLeague SubLeague { get; set; }

    public int? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    //Factory Method
    public static UserSetting Create(string userId, int subLeagueId, int? subscriptionId)
    {
        return new UserSetting(userId, subLeagueId, subscriptionId);
    }
}

