namespace Domain.Contracts;
public class GenericIconInfoContract
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? iconUrl { get; set; }
}
public class GenericAppDocumentContract: GenericIconInfoContract
{
    public string? value { get; set; }
}

public sealed class GenericAppDocumentStatusContract : GenericAppDocumentContract
{
    public bool status { get; set; }
}

public sealed class LeagueDetailContract : GenericIconInfoContract
{
    public int minRange { get; set; }
    public int maxRange { get; set; }
    public string myRanking { get; set; } = null!;
    public List<GenericIconInfoContract> point { get; set; } = new();

}
public sealed class WarningContract
{
    public int id { get; set; }
    public string title { get; set; } = null!;
    public string description { get; set; } = null!;
    public string createdAt { get; set; }
}