using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class MatchJoinRequest : BaseAuditableEntity
{
    internal MatchJoinRequest(string createdBy, StatusEnum requestStatus, string memberId, DateTime created)
    {
        MemberId = memberId;
        RequestStatus = requestStatus;
        Created = created;
        CreatedBy = createdBy;
    }

    //Properties
    public string MemberId { get; set; } = null!;
    public StatusEnum RequestStatus { get; set; }

    //Foreign Key
    public int TennisMatchId { get; private set; }
    public TennisMatch TennisMatch { get; private set; } = null!;

    /// <summary>
    /// Notifications List
    /// </summary>
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    //Methods
    public void setMatchRefrence(TennisMatch tennisMatch)
    {
        TennisMatch = tennisMatch;
    }

    public void UpdateStatus(StatusEnum requestStatus)
    {
        RequestStatus = requestStatus;
    }

    public void SendNotification(string title, string description, string participantId, int? redirectId, int matchCategory, DateTime created, string createdBy)
    {
       var Notifications = new Notification(this.Id, title, description, participantId,
       redirectId, false, false, NotificationTypeEnum.Clickable, NotificationRedirectEnum.ParticipantRequest, (GameTypeEnum)matchCategory, created, createdBy);

        //Send Notification
        _notifications.Add(Notifications);
    }
}