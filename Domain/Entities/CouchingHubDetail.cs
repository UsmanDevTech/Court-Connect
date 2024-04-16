using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class CouchingHubDetail: BaseAuditableEntity
{
    internal CouchingHubDetail(MediaTypeEnum type, string? title, string? description,
        string? thumbnail, string? url, long? timeLength)
    {
        Type = type;
        Title = title;
        Description = description;
        Thumbnail = thumbnail;
        Url = url;
        TimeLength = timeLength;
    }

    public MediaTypeEnum Type { get; private set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Thumbnail { get; private set; }
    public string? Url { get; private set; }
    public long? TimeLength { get; private set; }
   

    /// <summary>
    /// Foreign reference
    /// </summary>
    public int CouchingHubId { get; private set;}
    public CouchingHub CouchingHub { get; private set; } = null!;

    //Method

    public void UpdateType(MediaTypeEnum type)
    {
        Type = type;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }
    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateThumbnail(string thumbnail)
    {
        Thumbnail = thumbnail;
    }

    public void UpdateUrl(string url)
    {
        Url = url;
    }
    public void setCouchingHubObjectReference(CouchingHub couchingHub)
    {
        CouchingHub = couchingHub;
    }
}
