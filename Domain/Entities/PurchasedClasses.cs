using Domain.Common;

namespace Domain.Entities;

public class PurchasedClasses: BaseAuditableEntity
{
    internal PurchasedClasses(string createdBy, int couchingHubId, double price, double priceAfterDiscount,
      bool isDiscountAvailable, double? discount, double? stripeFee, string paymentId, DateTime created)
    {
        CreatedBy = createdBy;
        CouchingHubId = couchingHubId;
        Price = price;
        PriceAfterDiscount = priceAfterDiscount;
        IsDiscountAvailable = isDiscountAvailable;
        Discount = discount;
        StripeFee = stripeFee;
        PaymentId = paymentId;
        Created = created;
    }

    //Properties
    public double Price { get; set; }
    public double PriceAfterDiscount { get; set; }
    public bool IsDiscountAvailable { get; set; }
    public double? Discount { get; set; }
    
    public double? StripeFee { get; set; }
    public string PaymentId { get; set; }

    // Foreign key
    public int CouchingHubId { get; set; }
    public CouchingHub CouchingHub { get; set; } = null!;

    //Factory Method
    public void setCouchingHubRefrence(CouchingHub hub)
    {
        CouchingHub = hub;
    }
}
