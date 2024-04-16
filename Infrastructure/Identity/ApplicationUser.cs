using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public UserTypeEnum LoginRole { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Fullname { get; set; }
    public DateTime DateOfBrith { get; set; } 
    public GenderTypeEnum Gender { get; set; }
    public LevelEnum Level { get; set; }
    public PlayingTimeEnum PlayingTennis { get; set; }
    public MonthPlayTimeEnum MonthPlayTime { get; set; }
    public DTBEnum DtbPerformanceClass { get; set; }
    public string? ClubName { get; set; }
    public Point Location { get; set; } = null!;
    public double Points { get; set; } = 0;
    public string Address { get; set; } = null!;
    public double Radius { get; set; }
    public string? About { get; set; }
    public double? Rating { get; set; }
    public int ReviewPersonCount { get; set; } = 0;
    public bool isSubscriptionPurchased { get; set; }
   
    public DateTime CreatedAt { get; set; }
    public string? BlockReason { get; set; }
    public string? TimezoneId { get; set; }
    public string? fcmToken { get; set; }
    public StatusEnum AccountStatus { get; set; }

    //Delete Account 
    public DateTime? DeleteAccountRequestedAt { get; set; }
    public bool RequestForPermanentDelete { get; set; } = false;


}
