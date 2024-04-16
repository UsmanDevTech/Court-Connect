using Domain.Enum;
namespace Domain.Contracts;
public sealed class NotificationContract
{
    public int id { get; set; }
    public GenericUserDetailContract notifyBy { get; set; } = null!;
    public GenericUserDetailContract notifyTo { get; set; } = null!;
    public string body { get; set; }
    public string title { get; set; } = null!;
    public string createdAt { get; set; } = null!;
    public NotificationTypeEnum type { get; set; }
    public StatusEnum status { get; set; }
    public NotificationRedirectEnum redirectType { get; set; }
    public int? redirectId { get; set; }
    public bool isSeen { get; set; }
    public int? MatchJoinRequestId { get; set; }
    public GameTypeEnum matchCategory { get; set; }
}
