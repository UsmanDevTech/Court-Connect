using Domain.Enum;

namespace Domain.Contracts;

public class SubscriptionContract
{
    public int id { get; set; }
    public string title { get; set; } = null!;
    public double? price { get; set; }
    public double? priceAfterDiscount { get; set; }
    public bool isDiscountAvailable { get; set; }
    public double? discount { get; set; }
    public SubscriptionTypeEnum subscriptionType { get; set; }
    public DurationTypeEnum durationType { get; set; }

    public bool isFreeRankedGameUnlimited { get; set; }
    public bool isFreeUnrankedGameUnlimited { get; set; }
    public int? freeRankedGames { get; set; }
    public int? freeUnrankedGames { get; set; }

    public double? costPerRankedGame { get; set; }
    public double? costPerUnrankedGame { get; set; }

    public bool isFreeCouchingContentAvailable { get; set; }
    public bool isPaidCouchingContentAvailable { get; set; }

    public bool isRatingAvailable { get; set; }
    public bool isMatchBalanceAvailable { get; set; }
    public bool isScoreAvailable { get; set; }
    public bool isReviewAvailable { get; set; }
    public string? created { get; set; } = "";
}

public sealed class SubscriptionDtlsContract: SubscriptionContract
{
    public int subscriptionId { get; set; }
    public int? remainingFreeUnrankedMatches { get; set; }
    public int? remainingFreeRankedMatches { get; set; }
    public UserPackageStatusEnum subscriptionPurchasedStatus { get; set; }
}

public sealed class SubscriptionPckContract : SubscriptionContract
{
    public List<SubscriptionPointDtlsContract>? headings { get; set; }
}

public sealed class SubscriptionPointDtlsContract
{
    public string Title { get; set; } = null!;
    public List<GenericIconInfoContract> points { get; set; } = null!;
}

public class PurchaseSubscriptionContract
{
    public int PackageId { get; set; }
    public string PaymentId { get; set; } = null!;
    public double AmountPaid { get; set; }
}

public class AllSubscriptionContract: SubscriptionContract
{
    public bool deleted { get; set; }
 }


