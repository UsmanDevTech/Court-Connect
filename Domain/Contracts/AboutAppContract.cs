
namespace Domain.Contracts;

public class AboutAppContract
{
    public GenericAppDocumentContract? website { get; set; }
    public List<GenericAppDocumentContract> faq { get; set; } = new();
    public List<GenericAppDocumentContract> contactUs { get; set; } = new();
}
