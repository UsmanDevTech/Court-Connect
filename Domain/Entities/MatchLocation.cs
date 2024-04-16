
using Domain.Common;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class MatchLocation : BaseAuditableEntity
{
    internal MatchLocation(string createdBy, string userId, Point location
        ,string address ,DateTime created)
    {
        UserId = userId;
        Address = address;
        Location = location;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public Point Location { get; set; }
    public string Address { get; set; } = null!;
    public string UserId { get; set; } = null!;

    //Foreign Key
    public int TennisMatchId { get; private set; }
    public TennisMatch TennisMatch { get; private set; } = null!;

    //Factory Method

    public void setTennisMatchReference(TennisMatch tennisMatch)
    {
        TennisMatch = tennisMatch;
    }

    public void UpdateLocation(string address, double latitute, double longitute)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Point matchLocation = geometryFactory.CreatePoint(new Coordinate(longitute, latitute));

        Address = address;
        Location = matchLocation;
    }
}
