using Domain.Common;

namespace Domain.Entities;
public class PurchasedMatch : BaseAuditableEntity
{
    internal PurchasedMatch(string createdBy, double price,
        double? tax, double? stripeFee, string paymentId, DateTime created)
    {
        CreatedBy = createdBy;
        Price = price;
        Tax = tax;
        StripeFee = stripeFee;
        PaymentId = paymentId;
        Created = created;
    }

    //Properties
    public double Price { get; set; }
    public double? Tax { get; set; }
    public double? StripeFee { get; set; }
    public string PaymentId { get; set; }

    // Foreign key
    public int TennisMatchId { get; set; }
    public TennisMatch TennisMatch { get; set; } = null!;

    public void setTennisMatchRefrence(TennisMatch match)
    {
        TennisMatch = match;
    }
}