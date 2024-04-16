using Domain.Enum;

namespace Domain.Contracts;
public class GenericUserDetailContract
{
    public string id { get; set; } = null!;
    public string name { get; set; } = null!;
    public string profilePic { get; set; } = null!;
    public string? phone { get; set; }
    public string? email { get; set; }
}

public class UserProfileInfoDetailContract : GenericUserDetailContract
{
    public GenderTypeEnum gender { get; set; }
    public LevelEnum level { get; set; }
    public PlayingTimeEnum playingTennis { get; set; }
    public MonthPlayTimeEnum monthPlayTime { get; set; }
    public DTBEnum dtbPerformanceClass { get; set; }
    public UserTypeEnum loginRole { get; set; }
    public string dateOfBirth { get; set; } = null!;
    public string? clubName { get; set; }
    public string address { get; set; } = null!;
    public double latitute { get; set; }
    public double longitute { get; set; }
    public double points { get; set; } = 0;
    public string? about { get; set; }
    public double? ratings { get; set; }
    public double radius { get; set; }
    public int reviewedPersonCount { get; set; }
    public bool isEmailConfirmed { get; set; }
    public List<string>? interests { get; set; }
    public LeagueDetailContract league { get; set; } = null!;
    public bool isSubscriptionPurchased { get; set; }
    public SubscriptionDtlsContract? purchasedSubscription { get; set; }
    public List<WarningContract>? warnings { get; set; } = new();
}

public sealed class LeagueRankingUserContract : GenericUserDetailContract
{
    public int points { get; set; }
    public string? rank { get; set; }
}


public sealed class UserProfileInfoDetailStatusContract : UserProfileInfoDetailContract
{
    public int rankedMatches { get; set; }
    public int unrankedMatches { get; set; }
    public string createdAt { get; set; }
    public DateTime createdAtDateTime { get; set; }
    public string? blockReason { get; set; }
    public StatusEnum accountStatus { get; set; }
}

public sealed class MatchRequestUsersContract : GenericUserDetailContract
{
    public StatusEnum requestStatus { get; set; }
    public string requestCreatedAt { get; set; } = null!;
    public bool isRequested { get; set; }
    public double point { get; set; }
}

public sealed class CouchingHubPurchasedHistory : GenericUserDetailContract
{
    public string date { get; set; } = null!;
    public double price { get; set; }
}

public sealed class UserWarningContract
{
    public int id { get; set; }
    public GenericUserDetailContract reportedTo { get; set; } = new();
    public GenericUserDetailContract reportedBy { get; set; } = new();
    public string title { get; set; } = null!;
    public string description { get; set; }
}