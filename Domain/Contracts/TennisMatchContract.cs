
using Domain.Enum;

namespace Domain.Contracts;

public class ttt
{
    public string? profilePic { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
    public string password { get; set; } = null!;
    public string? phoneNumber { get; set; }
    public string dateOfBirth { get; set; } = null!;
    public int gender { get; set; }
    public int level { get; set; }
    public int playingTennis { get; set; }
    public int playInMonth { get; set; }
    public int dtbPerformanceClass { get; set; }
    public string? clubName { get; set; }
    public double latitute { get; set; }
    public double longitute { get; set; }
    public string address { get; set; } = null!;
    public double radius { get; set; }


}

public class TennisMatchContract
{
    public int id { get; set; }
    public bool isMyMatch { get; set; }
    public bool isJoined { get; set; }
    public string thumbnail { get; set; } = null!;
    public MatchStatusEnum status { get; set; }
    public string title { get; set; } = null!;
    public MatchTypeEnum matchType { get; set; }
    public GameTypeEnum matchCategory { get; set; }
    public LevelEnum level { get; set; }
    public string address { get; set; } = null!;
    public double latitute { get; set; }
    public double longitute { get; set; }
    public string startDate { get; set; } = null!;
    public string startDateTime { get; set; } = null!;
    public DateTime startDateTimeDate { get; set; }


}
public sealed class ProfileScoreContract
{
    public int matchId { get; set; }
    public string startDateTime { get; set; } = null!;
    public LeagueRankingUserContract creator { get; set; } = new();
    public ProfileTeamScoreContract teamA { get; set; } = null!;
    public ProfileTeamScoreContract teamB { get; set; } = null!;
}
public sealed class TennisMatchDtlsContract : TennisMatchContract
{
    public string? description { get; set; }
    public LeagueRankingUserContract? myPartner { get; set; }
    public LeagueRankingUserContract creator { get; set; } = new();
    public List<LeagueRankingUserContract> participants { get; set; } = new();
    public List<MatchRequestUsersContract> requestedUsers { get; set; } = new();
    public List<TeamContract> teamA { get; set; } = null!;
    public List<TeamContract> teamB { get; set; } = null!;
}

public sealed class ScoreConfirmationContract
{
    public int id { get; set; }
    public string title { get; set; } = null!;
    public MatchTypeEnum matchType { get; set; }
    public GameTypeEnum matchCategory { get; set; }
    public List<TeamContract> teamA { get; set; } = null!;
    public List<TeamContract> teamB { get; set; } = null!;
    public List<int?> TeamAScore { get; set; } = new();
    public List<int?> TeamBScore { get; set; } = new();
}

public sealed class ScoreCalculationContract
{
    public int matchId { get; set; }
    public TeamScoreContract teamA { get; set; } = new();
    public TeamScoreContract teamB { get; set; } = new();
}

public sealed class MatchScoreCalculationContract
{
    public int matchId { get; set; }
    public TeamScoreContract? teamA { get; set; }
    public TeamScoreContract? teamB { get; set; }
}


public class TeamContract
{
    public bool isWon { get; set; } = false;
    public LeagueRankingUserContract member { get; set; } = null!;
    public string teamCode { get; set; } = null!;
}

public sealed class TeamScoreContract
{
    public bool isMatchWon { get; set; }
    public List<int?>? scoreList { get; set; }
    public string teamCode { get; set; } = null!;
    public List<TeamUserContract> members { get; set; } = null!;

}

public sealed class ProfileTeamScoreContract
{
    public bool isMatchWon { get; set; }
    public List<int?> scoreList { get; set; }
    public string teamCode { get; set; } = null!;
    public List<LeagueRankingUserContract> members { get; set; } = null!;

}

public sealed class TeamUserContract
{
    public bool isRated { get; set; } = false;
    public double? previousPoint { get; set; }
    public double? addMinusPoint { get; set; }
    public double? newPoint { get; set; }
    public LeagueRankingUserContract user { get; set; } = null!;
}


public sealed class TennisJoinedCreatedMatchContract
{
    public List<TennisMatchContract> created { get; set; } = new();
    public List<TennisMatchContract> joined { get; set; } = new();

}

public sealed class RatingContract
{
    public string memberId { get; set; } = null!;
    public double forehead { get; set; }
    public double backhand { get; set; }
    public double serve { get; set; }
    public double fairness { get; set; }
    public string comment { get; set; } = null!;
}


public sealed class MatchRatingContract
{
    public double overallRating { get; set; }
    public int reviewsCount { get; set; }
    public List<MatchRatingProfileContract> ratings { get; set; } = new();
}


public class LineRankingChart
{
    public int year { get; set; }
    public int MonthNumber { get; set; }
    public string month { get; set; } = null!;
    public double rating { get; set; }
}
public sealed class MatchBalanceContract
{
    public int wonMatches { get; set; }
    public int lossMatches { get; set; }
    public int setWon { get; set; }
    public int setLoss { get; set; }
    public int gameWon { get; set; }
    public int gameLoss { get; set; }
    public double pointGainedLoss { get; set; }
}
public class MatchRatingProfileContract
{
    public LeagueRankingUserContract user { get; set; } = null!;
    public double totalRating { get; set; }
    public double forehead { get; set; }
    public double backhand { get; set; }
    public double serve { get; set; }
    public double fairness { get; set; }
    public string comment { get; set; } = null!;
    public string created { get; set; } = null!;
}
public class MatchOwnerDetailContract
{
    public string id { get; set; } = null!;
    public string name { get; set; } = null!;
    public string profilePic { get; set; } = null!;
    public string? phone { get; set; }
    public string? email { get; set; }
    public int totalMatches { get; set; } = 0;
}

public sealed class TennisMatchDtlsByUserContract : TennisMatchContract
{
    public string? description { get; set; }
    public string created { get; set; } = null!;
    public bool deleted { get; set; } = false;
    public string? blockReason { get; set; }
    public LeagueRankingUserContract creator { get; set; } = new();
}

public sealed class LocationVerificationContract
{
    public int matchId { get; set; }
    public List<string> participants { get; set; } = null!;
}