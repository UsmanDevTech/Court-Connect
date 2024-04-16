using Domain.Abstraction;
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class Notification : BaseAuditableEntity, ISoftDelete
{
    public Notification(int? matchJoinRequestId, string title, string description, string notifyTo, int? redirectId, bool isSeen,
        bool deleted, NotificationTypeEnum type, NotificationRedirectEnum redirectType,
        GameTypeEnum matchCategory, DateTime created, string? createdBy)
    {
        MatchJoinRequestId = matchJoinRequestId;
        Title = title;
        Description = description;
        NotifyTo = notifyTo;
        RedirectId = redirectId;
        IsSeen = isSeen;
        Deleted = deleted;
        Type = type;
        RedirectType = redirectType;
        MatchCategory = matchCategory;
        Created = created;
        CreatedBy = createdBy;
    }

    //Properties
    public string Title { get; private set; } = null!;
    public string Description { get; private set; }
    public string NotifyTo { get; private set; } = null!;
    public int? RedirectId { get; private set; }
    public bool IsSeen { get; private set; }

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

    public NotificationTypeEnum Type { get; private set; }
    public NotificationRedirectEnum RedirectType { get; private set; }
    public GameTypeEnum MatchCategory { get; private set; }

    //Foreign Key
    public int? MatchJoinRequestId { get; private set; }
    public MatchJoinRequest? MatchJoinRequest { get; private set; }

    
    //Method
    public void UpdateReadStatus(bool isSeen)
    {
        IsSeen = isSeen;
    }

    public void UpdateRedirectType(NotificationRedirectEnum redirectType)
    {
        RedirectType = redirectType;
    }
    public void UpdateCreatedAt(DateTime created)
    {
        Created = created;
    }

    public static Notification Create(int? matchJoinRequestId, string title, string description, string notifyTo, int? redirectId,
        NotificationTypeEnum type, NotificationRedirectEnum redirectType, GameTypeEnum matchCategory, DateTime created, string? createdBy)
    {
        return new Notification(matchJoinRequestId, title, description, notifyTo, redirectId, false, false, type, redirectType, matchCategory, created, createdBy);
    }
}