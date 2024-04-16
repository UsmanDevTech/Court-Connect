using Domain.Abstraction;
using Domain.Common;
using Domain.Enum;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class TennisMatch : BaseAuditableEntity, ISoftDelete
{
    private TennisMatch(string title, string? description, string matchImage,
     MatchTypeEnum type, GameTypeEnum matchCategory, Point location, string address,
     DateTime matchDateTime, MatchStatusEnum status, bool isMembersLimitFull, bool deleted, string createdBy, DateTime created)
    {
        Title = title;
        Description = description;
        MatchImage = matchImage;
        Type = type;
        MatchCategory = matchCategory;
        Location = location;
        Address = address;
        MatchDateTime = matchDateTime;
        Status = status;
        IsMembersLimitFull = isMembersLimitFull;
        Deleted = deleted;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string MatchImage { get; set; } = null!;
    public MatchTypeEnum Type { get; set; }
    public GameTypeEnum MatchCategory { get; set; }
    public Point Location { get; set; }
    public string Address { get; set; }
    public DateTime MatchDateTime { get; set; }
    public MatchStatusEnum Status { get; set; }
    public string? BlockReason { get; set; }
    public bool IsMembersLimitFull { get; set; }

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

    /// <summary>
    /// Tennis match members
    /// </summary>
    private readonly List<MatchMember> _matchMembers = new();
    public IReadOnlyCollection<MatchMember> MatchMembers => _matchMembers.AsReadOnly();

    /// <summary>
    /// Tennis match members
    /// </summary>
    private readonly List<PurchasedMatch> _purchasedMatches = new();
    public IReadOnlyCollection<PurchasedMatch> purchasedMatches => _purchasedMatches.AsReadOnly();

    /// <summary>
    /// Join match request
    /// </summary>
    private readonly List<MatchJoinRequest> _matchJoinRequests = new();
    public IReadOnlyCollection<MatchJoinRequest> MatchJoinRequests => _matchJoinRequests.AsReadOnly();


    /// <summary>
    /// Tennis match reviews
    /// </summary>
    private readonly List<MatchReview> _matchReviews = new();
    public IReadOnlyCollection<MatchReview> MatchReviews => _matchReviews.AsReadOnly();

    /// <summary>
    /// Chat head
    /// </summary>
    private readonly List<ChatHead> _chatHeads = new();
    public IReadOnlyCollection<ChatHead> ChatHeads => _chatHeads.AsReadOnly();

    /// <summary>
    /// Temporary Score
    /// </summary>
    private readonly List<TemporaryMatchScore> _temporaryMatchScores = new();
    public IReadOnlyCollection<TemporaryMatchScore> TemporaryMatchScores => _temporaryMatchScores.AsReadOnly();

    /// <summary>
    /// Match Location
    /// </summary>
    private readonly List<MatchLocation> _matchLocations = new();
    public IReadOnlyCollection<MatchLocation> MatchLocations => _matchLocations.AsReadOnly();



    //Factory Method
    public static TennisMatch Create(string title, string? description, string matchImage,
     MatchTypeEnum type, GameTypeEnum matchCategory, double latitute, double longitute, string address,
     DateTime matchDateTime, MatchStatusEnum status, bool isMembersLimitFull, string createdBy, DateTime created)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point matchLocation = geometryFactory.CreatePoint(new Coordinate(longitute, latitute));

        var match = new TennisMatch(title, description, matchImage, type, matchCategory,
            matchLocation, address, matchDateTime, status, isMembersLimitFull, false, createdBy, created);

        match.AddMatchMember(createdBy, createdBy, created, JoinTypeEnum.None, "");

        return match;
    }

    public void AddMatchMember(string createdBy, string memberId, DateTime createdAt, JoinTypeEnum joinType, string? matchTeamCode)
    {
        if (matchTeamCode != "")
        {
            var MatchMember = new MatchMember(createdBy, matchTeamCode, false, 0, 0, 0, 0, 0, 0, memberId, createdAt, joinType);
            MatchMember.setMatchRefrence(this);

            _matchMembers.Add(MatchMember);
        }
        else
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var teamCode = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var MatchMember = new MatchMember(createdBy, teamCode, false, 0, 0, 0, 0, 0, 0, memberId, createdAt, joinType);
            MatchMember.setMatchRefrence(this);
            //Add Match Member
            _matchMembers.Add(MatchMember);
        }
    }

    public void SendParticipantRequest(string createdBy, string memberId, string description, int matchCategory, DateTime createdAt)
    {
        var MemberJoinRequest = new MatchJoinRequest(createdBy, StatusEnum.Pending, memberId, createdAt);
        MemberJoinRequest.setMatchRefrence(this);

        //Add Match Member
        _matchJoinRequests.Add(MemberJoinRequest);

        //Send Notification
        MemberJoinRequest.SendNotification("MemberJoinRequest", description, memberId, this.Id, matchCategory, createdAt, createdBy);
    }

    public void UpdateLocation(string createdBy, string userId, double latitute, double longitute, string address, DateTime createdAt)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point matchLocation = geometryFactory.CreatePoint(new Coordinate(longitute, latitute));

        var MemberJoinRequest = new MatchLocation(createdBy, userId, matchLocation, address, createdAt);
        MemberJoinRequest.setTennisMatchReference(this);

        //Add Match Location
        _matchLocations.Add(MemberJoinRequest);
    }
    //Update match
    public void UpdateMatch(string title, string? description, string matchImage,
     MatchTypeEnum type, GameTypeEnum matchCategory, double latitute, double longitute, string address,
     DateTime matchDateTime, DateTime created)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point matchLocation = geometryFactory.CreatePoint(new Coordinate(longitute, latitute));

        Title = title;
        Description = description;
        MatchImage = matchImage;
        Type = type;
        MatchCategory = matchCategory;
        Location = matchLocation;
        Address = address;
        MatchDateTime = matchDateTime;
        Modified = created;
    }

    public void AddMatchReview(string createdBy, string notificationFor, double forehand,
        double backhand, double serve, double fairness, string comment, DateTime created)
    {
        var MatchReview = new MatchReview(createdBy, notificationFor, forehand, backhand, serve, fairness, comment, created);
        MatchReview.setTennisMatchReference(this);

        _matchReviews.Add(MatchReview);
    }

    public void AddMatchPurchase(string createdBy, double price, double? tax,
       double stripeFee, string PaymentIntentId, DateTime created)
    {
        var MatchPurchase = new PurchasedMatch(createdBy, price, tax, stripeFee, PaymentIntentId, created);
        MatchPurchase.setTennisMatchRefrence(this);

        _purchasedMatches.Add(MatchPurchase);
    }

    public void RemoveMember(MatchMember matchMember)
    {
        _matchMembers.Remove(matchMember);
    }

    public void updateMatchStatus(MatchStatusEnum status)
    {
        Status = status;
    }

    public void RemoveTempScore(TemporaryMatchScore match)
    {
        _temporaryMatchScores.Remove(match);
    }

    public void updateStatus(MatchStatusEnum satus)
    {
        Status = satus;
    }

    public void updateMatchDetail(string title, string? description)
    {
        Title = title;
        Description = description;
    }
    public void updateLocation(double latitute, double longitute, string address)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point matchLocation = geometryFactory.CreatePoint(new Coordinate(longitute, latitute));

        Location = matchLocation;
        Address = address;
    }

    public void updatePicture(string matchImage)
    {
        MatchImage = matchImage;
    }
}

