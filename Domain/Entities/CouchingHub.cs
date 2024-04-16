using Domain.Abstraction;
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class CouchingHub : BaseAuditableEntity, ISoftDelete
{
    private CouchingHub(string title, int couchingHubCategoryId, CouchingHubContentTypeEnum type, double? price, string createdBy, DateTime created)
    {
        CouchingHubCategoryId = couchingHubCategoryId;
        Title = title;
        Type = type;
        Price = price;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string Title { get; set; } = null!;
    public CouchingHubContentTypeEnum Type { get; set; }
    public double? Price { get; set; }


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
    //Foreign Key
    public int CouchingHubCategoryId { get; private set; }
    public CouchingHubCategory CouchingHubCategory { get; private set; } = null!;

    /// <summary>
    /// Couching Hub Detail
    /// </summary>
    private readonly List<CouchingHubDetail> _couchingHubDetails = new();
    public IReadOnlyCollection<CouchingHubDetail> CouchingHubDetails => _couchingHubDetails.AsReadOnly();

    /// <summary>
    /// Couching Hub Benifits
    /// </summary>
    private readonly List<CouchingHubBenifit> _couchingHubBenifits = new();
    public IReadOnlyCollection<CouchingHubBenifit> couchingHubBenifits => _couchingHubBenifits.AsReadOnly();

    /// <summary>
    /// Purchased Classes
    /// </summary>
    private readonly List<PurchasedClasses> _purchasedClassess = new();
    public IReadOnlyCollection<PurchasedClasses> PurchasedClassess => _purchasedClassess.AsReadOnly();


    //Factory Method
    public static CouchingHub Create(string title, int couchingHubCategoryId, CouchingHubContentTypeEnum type, double? price, string createdBy, DateTime created)
    {
        return new CouchingHub(title, couchingHubCategoryId, type, price, createdBy, created);
    }
    
    public void UpdatePrice(double price)
    {
        Price = price;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }
    public void AddCouchingPurchase(string createdBy, int couchingHubId, double price,
     double stripeFee, string PaymentIntentId, DateTime created)
    {
        var ContentPurchase = new PurchasedClasses(createdBy, couchingHubId, price, price, false, 0
            , stripeFee, PaymentIntentId, created);
        ContentPurchase.setCouchingHubRefrence(this);

        _purchasedClassess.Add(ContentPurchase);
    }

    public void AddCouchingHubBenifits(List<string> benifits, DateTime created)
    {
        if (benifits != null && benifits.Count > 0)
        {
            foreach (var benifit in benifits)
            {
                //Add Couching Hub Benifits
                var couchingHubBenifits = new CouchingHubBenifit(null, benifit, created);
                couchingHubBenifits.setCouchingHubObjectRefrence(this);

                _couchingHubBenifits.Add(couchingHubBenifits);
            }
        }
    }

    public void AddCouchingHubContent(MediaTypeEnum type, string? title,
        string? description, string? thumbnail, string? url, long? time)
    {
        var couchingHubContent = new CouchingHubDetail(type, title, description, thumbnail, url, time);
        couchingHubContent.setCouchingHubObjectReference(this);

        _couchingHubDetails.Add(couchingHubContent);
    }


}
