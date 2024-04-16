using Domain.Common;

namespace Domain.Entities;

public class TemporaryMatchScore : BaseAuditableEntity
{
    private TemporaryMatchScore(int? teamAScoreOne, int? teamAScoreTwo, int? teamAScoreThree,
        int? teamBScoreOne, int? teamBScoreTwo, int? teamBScoreThree, int tennisMatchId)
    {
        TennisMatchId = tennisMatchId;
        TeamAScoreOne = teamAScoreOne;
        TeamAScoreTwo = teamAScoreTwo;
        TeamAScoreThree = teamAScoreThree;
        TeamBScoreOne = teamBScoreOne;
        TeamBScoreTwo = teamBScoreTwo;
        TeamBScoreThree = teamBScoreThree;
    }

    //Properties
    public int? TeamAScoreOne { get; set; }
    public int? TeamAScoreTwo { get; set; }
    public int? TeamAScoreThree { get; set; }

    public int? TeamBScoreOne { get; set; }
    public int? TeamBScoreTwo { get; set; }
    public int? TeamBScoreThree { get; set; }


    //Foreign Key
    public int TennisMatchId { get; private set; }
    public TennisMatch TennisMatch { get; private set; } = null!;

    //Methods

    public static TemporaryMatchScore Create(int? teamAScoreOne, int? teamAScoreTwo, int? teamAScoreThree,
        int? teamBScoreOne, int? teamBScoreTwo, int? teamBScoreThree, int tennisMatchId)
    {

        return new TemporaryMatchScore(teamAScoreOne, teamAScoreTwo, teamAScoreThree, teamBScoreOne,
            teamBScoreTwo, teamBScoreThree, tennisMatchId);
    }

}
