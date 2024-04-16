namespace Domain.Enum;
public enum NotificationTypeEnum
{
    Simple = 0,
    Clickable = 1,
    Pending,
    Accepted,
    Rejected,
    Closed,
}

public enum NotificationRedirectEnum
{
    None = 0,
    ParticipantRequest = 1,
    MatchDetail = 2,
    ScoreVerification = 3,
    ChatMessage = 4,
}