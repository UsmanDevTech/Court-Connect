
namespace Domain.Enum;

public enum MatchStatusEnum
{
    Initial = 0,
    Waiting = 1,
    ReadyToStart = 2,
    Started = 3,
    Completed = 4,
    Rated = 5,
    Cancelled = 6,
    Expired = 7,
    InsufficientParticipant = 8,
}
