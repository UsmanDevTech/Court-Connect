
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class MatchMember : BaseAuditableEntity
{
    internal MatchMember(string createdBy, string teamCode,
        bool isMatchWon, int? setOnePoint, int? setTwoPoint, int? setThreePoint,
        double? previousPoint, double? gamePoint, double? newPoints,
        string memberId, DateTime created,  JoinTypeEnum matchJoinType)
    {
        MemberId = memberId;
        TeamCode = teamCode;
        IsMatchWon = isMatchWon;
        SetOnePoint = setOnePoint;
        SetTwoPoint = setTwoPoint;
        SetThreePoint = setThreePoint;
        PreviousPoint = previousPoint;
        GamePoint = gamePoint;
        NewPoints = newPoints;
        Created = created;
        CreatedBy = createdBy;
        IsScoreApproved = StatusEnum.Pending;
        MatchJoinType = matchJoinType;
    }

    //Properties
    public bool IsMatchWon { get; set; }
    public StatusEnum IsScoreApproved { get; set; }
    public JoinTypeEnum MatchJoinType { get; set; }

    public int? SetOnePoint { get; set; }
    public int? SetTwoPoint { get; set; }
    public int? SetThreePoint { get; set; }

    public double? PreviousPoint { get; set; }
    public double? GamePoint { get; set; }
    public double? NewPoints { get; set; }

    public string MemberId { get; set; } = null!;
    public string? TeamCode { get; set; }


    //Foreign Key
    public int TennisMatchId { get; private set; }
    public TennisMatch TennisMatch { get; private set; } = null!;

    //Methods
    public void setMatchRefrence(TennisMatch tennisMatch)
    {
        TennisMatch = tennisMatch;
    }

    public void updateIsMatchWon(bool isMatchWon)
    {
        IsMatchWon = isMatchWon;
    }

    public void updateIsScored(StatusEnum isScoreApproved)
    {
        IsScoreApproved = isScoreApproved;
    }

    
}
