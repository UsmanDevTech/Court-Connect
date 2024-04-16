using Domain.Abstraction;
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class Subscription : BaseAuditableEntity, ISoftDelete
{
    private Subscription(string title, double? price, double? priceAfterDiscount,
       bool isDiscountAvailable, double? discount, SubscriptionTypeEnum subscriptionType, DurationTypeEnum durationType,
       double? costPerRankedGame, double? costPerUnrankedGame,
       bool isFreeRankedUnlimited, bool isFreeUnrankedUnlimited,
       int freeRankedGames, int freeUnrankedGames,
       bool isPaidCouchingContentAvailable, bool isFreeCouchingContentAvailable,
       bool isRatingAvailable, bool isMatchBalanceAvailable, bool isScoreAvailable, bool isReviewsAvailable, string? productId,
       string? priceId, bool deleted, string createdBy, DateTime created)
    {
        Title = title;
        Price = price;
        PriceAfterDiscount = priceAfterDiscount;
        IsDiscountAvailable = isDiscountAvailable;
        Discount = discount;
        SubscriptionType = subscriptionType;
        DurationType = durationType;
       
        CostPerRankedGame = costPerRankedGame;
        CostPerUnrankedGame = costPerUnrankedGame;

        IsFreeRankedUnlimited = isFreeRankedUnlimited;
        IsFreeUnrankedUnlimited = isFreeUnrankedUnlimited;
        FreeRankedGames = freeRankedGames;
        FreeUnrankedGames = freeUnrankedGames;

        IsFreeCouchingContentAvailable = isFreeCouchingContentAvailable;
        IsPaidCouchingContentAvailable = isPaidCouchingContentAvailable;

        IsMatchBalanceAvailable = isMatchBalanceAvailable;
        IsRatingAvailable = isRatingAvailable;
        IsScoreAvailable = isScoreAvailable;
        IsReviewsAvailable = isReviewsAvailable;

        ProductId = productId;
        PriceId = priceId;
        Deleted = deleted;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string Title { get; set; } = null!;
    public double? Price { get; set; }
    public double? PriceAfterDiscount { get; set; }
    public bool IsDiscountAvailable { get; set; }
    public double? Discount { get; set; }
    public SubscriptionTypeEnum SubscriptionType { get; set; }
    public DurationTypeEnum DurationType { get; set; }

    public bool IsRankedUnlimited { get; set; }
    public bool IsUnrankedUnlimited { get; set; }
    public int? RankedGames { get; set; }
    public int? UnrankedGames { get; set; }

    public bool IsFreeRankedUnlimited { get; set; }
    public bool IsFreeUnrankedUnlimited { get; set; }
    public int FreeRankedGames { get; set; }
    public int FreeUnrankedGames { get; set; }

    public double? CostPerRankedGame { get; set; }
    public double? CostPerUnrankedGame { get; set; }

    public bool IsFreeCouchingContentAvailable { get; set; }
    public bool IsPaidCouchingContentAvailable { get; set; }

    public bool IsMatchBalanceAvailable { get; set; }
    public bool IsRatingAvailable { get; set; }
    public bool IsScoreAvailable { get; set; }
    public bool IsReviewsAvailable { get; set; }

    public string? ProductId { get; set; }
    public string? PriceId { get; set; }

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
    /// Subscription Headings
    /// </summary>
    private readonly List<SubscriptionHeading> _subscriptionHeadings = new();
    public IReadOnlyCollection<SubscriptionHeading> SubscriptionHeadings => _subscriptionHeadings.AsReadOnly();

    /// <summary>
    /// Subscription History
    /// </summary>
    private readonly List<SubscriptionHistory> _subscriptionHistories = new();
    public IReadOnlyCollection<SubscriptionHistory> SubscriptionHistories => _subscriptionHistories.AsReadOnly();

    /// <summary>
    /// User Setting
    /// </summary>
    private readonly List<UserSetting> _userSettings = new();
    public IReadOnlyCollection<UserSetting> UserSettings => _userSettings.AsReadOnly();

    //Factory Method
    public static Subscription Create(string title, double? price, double? priceAfterDiscount,
       bool isDiscountAvailable, double? discount, SubscriptionTypeEnum subscriptionType, DurationTypeEnum durationType,
       double? costPerRankedGame, double? costPerUnrankedGame,
       bool isFreeRankedUnlimited, bool isFreeUnrankedUnlimited,
       int freeRankedGames, int freeUnrankedGames,
       bool isPaidCouchingContentAvailable, bool isFreeCouchingContentAvailable,
       bool isRatingAvailable, bool isMatchBalanceAvailable, bool isScoreAvailable, bool isReviewsAvailable, string? productId,
       string? priceId, string createdBy, DateTime created)
    {
        return new Subscription(title, price, priceAfterDiscount, isDiscountAvailable, discount,
            subscriptionType, durationType,
            costPerRankedGame, costPerUnrankedGame, 
            isFreeRankedUnlimited, isFreeUnrankedUnlimited, freeRankedGames, freeUnrankedGames,
            isPaidCouchingContentAvailable, isFreeCouchingContentAvailable, isRatingAvailable,
            isMatchBalanceAvailable, isScoreAvailable, isReviewsAvailable, productId, priceId, false
            , createdBy, created);
    }

    public void SetSubscription(string title, double? price, double? priceAfterDiscount,
       bool isDiscountAvailable, double? discount,
       double? costPerRankedGame, double? costPerUnrankedGame,
       bool isFreeRankedUnlimited, bool isFreeUnrankedUnlimited,
       int freeRankedGames, int freeUnrankedGames,
       bool isPaidCouchingContentAvailable, bool isFreeCouchingContentAvailable,
       bool isRatingAvailable, bool isMatchBalanceAvailable, bool isScoreAvailable, bool isReviewsAvailable,
       string? priceId)
    {
        Title = title;
        Price = price;
        PriceAfterDiscount = priceAfterDiscount;
        IsDiscountAvailable = isDiscountAvailable;
        Discount = discount;
        CostPerRankedGame = costPerRankedGame;
        CostPerUnrankedGame = costPerUnrankedGame;
        IsFreeRankedUnlimited = isFreeRankedUnlimited;
        IsFreeUnrankedUnlimited = isFreeUnrankedUnlimited;
        FreeRankedGames = freeRankedGames;
        FreeUnrankedGames = freeUnrankedGames;
        IsPaidCouchingContentAvailable = isPaidCouchingContentAvailable;
        isFreeCouchingContentAvailable = isFreeCouchingContentAvailable;
        IsRatingAvailable = isRatingAvailable;
        IsMatchBalanceAvailable = isMatchBalanceAvailable;
        IsScoreAvailable = isScoreAvailable;
        IsReviewsAvailable = isReviewsAvailable;
        PriceId = PriceId;
    }
}
