using Domain.Common;

namespace Domain.Entities;

public class MatchReview : BaseAuditableEntity
{
    internal MatchReview(string createdBy, string reviewedTo, double forehand,
     double backhand, double serve, double fairness, string comment, DateTime created)
    {
        ReviewedTo = reviewedTo;
        Forehand = forehand;
        Backhand = backhand;
        Serve = serve;
        Fairness = fairness;
        Comment = comment;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string ReviewedTo { get; set; } = null!;
    public double Forehand { get; set; }
    public double Backhand { get; set; }
    public double Serve { get; set; }
    public double Fairness { get; set; }
    public string Comment { get; set; }

    //Foreign Key
    public int TennisMatchId { get; private set; }
    public TennisMatch TennisMatch { get; private set; } = null!;

    //Factory Method

    public void setTennisMatchReference(TennisMatch tennisMatch)
    {
        TennisMatch = tennisMatch;
    }
}
