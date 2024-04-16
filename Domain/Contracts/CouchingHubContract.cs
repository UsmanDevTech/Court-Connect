
using Domain.Enum;

namespace Domain.Contracts;

public class CouchingHubContract
{
    public int id { get; set; }
    public string title { get; set; } = null!;
    public CouchingHubContentTypeEnum type { get; set; }
    public double? price { get; set; }
    public bool isPurchased { get; set; }
    public GenericIconInfoContract category { get; set; } = null!;
    public CouchingHubDetailContract content { get; set; } = null!;
    public List<GenericIconInfoContract> benifits { get; set; } = new();
    public string date { get; set; } = null!;
}

public class CouchingHubDetailContract
{
    public int id { get; set; }
    public MediaTypeEnum type { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public string? thumbnail { get; set; }
    public string? url { get; set; }
    public long? timeLength { get; set; }
}


public class AdminCouchingHubContract
{
    public int id { get; set; }
    public string title { get; set; } = null!;
    public string description { get; set; } = null!;
    public MediaTypeEnum mediaType { get; set; }
    public string? thumbnail { get; set; }
    public string? url { get; set; }
    public CouchingHubContentTypeEnum type { get; set; }
    public double? price { get; set; }
    public int categoryId { get; set; }
    public string category { get; set; } = null!;
    public string date { get; set; } = null!;
    public bool deleted { get; set; }
}

public class CouchingHubCategoryContract
{
    public int id { get; set; }
    public string icon { get; set; }
    public string name { get; set; }
    public bool deleted { get; set; }
}
