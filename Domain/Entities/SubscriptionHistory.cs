using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class SubscriptionHistory : BaseAuditableEntity
{
    private SubscriptionHistory(string createdBy, int subscriptionId, double? price, double? priceAfterDiscount,
       bool isDiscountAvailable, double? discount,
       double? costPerRankedGame, double? costPerUnrankedGame,
       bool isFreeRankedUnlimited, bool isFreeUnrankedUnlimited,
       int freeRankedGames, int freeUnrankedGames,
       int remainingFreeRankedGames, int remainingFreeUnrankedGames,
       bool isPaidCouchingContentAvailable, bool isFreeCouchingContentAvailable,
        bool isRatingAvailable, bool isMatchBalanceAvailable, bool isScoreAvailable, bool isReviewsAvailable,
        DateTime expireAt, double? tax, double? stripeFee, string? paymentId, UserPackageStatusEnum subscriptionStatus, DateTime created)
    {
        CreatedBy = createdBy;
        SubscriptionId = subscriptionId;
        Price = price;
        PriceAfterDiscount = priceAfterDiscount;
        IsDiscountAvailable = isDiscountAvailable;
        Discount = discount;

        IsFreeRankedUnlimited = isFreeRankedUnlimited;
        IsFreeUnrankedUnlimited = isFreeUnrankedUnlimited;
        FreeRankedGames = freeRankedGames;
        FreeUnrankedGames = freeUnrankedGames;

        RemainingFreeRankedGames = remainingFreeRankedGames;
        RemainingFreeUnrankedGames = remainingFreeUnrankedGames;

        CostPerRankedGame = costPerRankedGame;
        CostPerUnrankedGame = costPerUnrankedGame;

        IsFreeCouchingContentAvailable = isFreeCouchingContentAvailable;
        IsPaidCouchingContentAvailable = isPaidCouchingContentAvailable;

        IsRatingAvailable = isRatingAvailable;
        IsMatchBalanceAvailable = isMatchBalanceAvailable;
        IsScoreAvailable = isScoreAvailable;
        IsReviewsAvailable = isReviewsAvailable;
        SubscriptionStatus = subscriptionStatus;
        ExpireAt = expireAt;
        Tax = tax;
        StripeFee = stripeFee;
        PaymentId = paymentId;
        Created = created;
    }

    public SubscriptionHistory() { }
    //Properties
    public double? Price { get; set; }
    public double? PriceAfterDiscount { get; set; }
    public bool IsDiscountAvailable { get; set; }
    public double? Discount { get; set; }

    public bool IsFreeRankedUnlimited { get; set; }
    public bool IsFreeUnrankedUnlimited { get; set; }
    public int FreeRankedGames { get; set; }
    public int FreeUnrankedGames { get; set; }

    public int RemainingFreeRankedGames { get; set; }
    public int RemainingFreeUnrankedGames { get; set; }


    public double? CostPerRankedGame { get; set; }
    public double? CostPerUnrankedGame { get; set; }

    public bool IsFreeCouchingContentAvailable { get; set; }
    public bool IsPaidCouchingContentAvailable { get; set; }

    public bool IsMatchBalanceAvailable { get; set; }
    public bool IsRatingAvailable { get; set; }
    public bool IsScoreAvailable { get; set; }
    public bool IsReviewsAvailable { get; set; }


    public UserPackageStatusEnum SubscriptionStatus { get; set; }
    public DateTime ExpireAt { get; set; }
    public double? Tax { get; set; }
    public double? StripeFee { get; set; }
    public string? PaymentId { get; set; }
  
    // Foreign key
    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; }

    //Factory Method
    public static SubscriptionHistory Create(string createdBy, int subscriptionId, double? price,
        double? priceAfterDiscount, bool isDiscountAvailable, double? discount, double? costPerRankedGame, double? costPerUnrankedGame,
        bool isFreeRankedUnlimited, bool isFreeUnrankedUnlimited,
        int freeRankedGames, int freeUnrankedGames,
        int remainingFreeRankedGames, int remainingFreeUnrankedGames,
        bool isPaidCouchingContentAvailable, bool isFreeCouchingContentAvailable,
        bool isRatingAvailable, bool isMatchBalanceAvailable, bool isScoreAvailable, bool isReviewsAvailable,
        DateTime expireAt, double? tax, double? stripeFee, string? paymentId, UserPackageStatusEnum subscriptionStatus, DateTime created)
    {
        return new SubscriptionHistory(createdBy, subscriptionId, price, priceAfterDiscount, isDiscountAvailable, discount,
            costPerRankedGame, costPerUnrankedGame,
            isFreeRankedUnlimited, isFreeUnrankedUnlimited, freeRankedGames, freeUnrankedGames,
            remainingFreeRankedGames, remainingFreeUnrankedGames,
            isPaidCouchingContentAvailable, isFreeCouchingContentAvailable, isRatingAvailable,
           isMatchBalanceAvailable, isScoreAvailable, isReviewsAvailable, expireAt, tax, stripeFee, paymentId, subscriptionStatus, created);
    }

    public void UpdateStatus(UserPackageStatusEnum status)
    {
        SubscriptionStatus = status;
    }
    public void UpdateRemainingCount(int type)
    {
        if (type == 0)
            RemainingFreeUnrankedGames += 1;
        else
            RemainingFreeRankedGames += 1;
    }
}
