
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    /// <summary>
    /// Setting
    /// </summary>

    DbSet<AppContent> AppContent { get; }
    DbSet<AccountOtpHistory> AccountOtpHistory { get; }

    /// <summary>
    /// Application User
    /// </summary>

    DbSet<UserInterest> UserInterests { get; }
    DbSet<UserSetting> UserSettings { get; }
    DbSet<ProfileWarning> ProfileWarnings { get; }


    /// <summary>
    /// League
    /// </summary>
    /// 
    DbSet<League> Leagues { get; }
    DbSet<SubLeague> SubLeagues { get; }
    DbSet<LeagueReward> LeagueRewards { get; }

    /// <summary>
    /// Subscription
    /// </summary>

    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionHeading> SubscriptionHeadings { get; }
    DbSet<SubscriptionPoint> SubscriptionPoints { get; }
    DbSet<SubscriptionHistory> SubscriptionHistories { get; }

    /// <summary>
    /// Couching Hub
    /// </summary>

    DbSet<CouchingHubCategory> CouchingHubCategories { get; }
    DbSet<CouchingHub> CouchingHubs { get; }
    DbSet<CouchingHubDetail> CouchingHubDetails { get; }
    DbSet<CouchingHubBenifit> CouchingHubBenifits { get; }
    DbSet<PurchasedClasses> PurchasedClassess { get; }

    /// <summary>
    /// Match Member
    /// </summary>
    DbSet<Notification> Notifications { get; }
    DbSet<MatchLocation> MatchLocations { get; }
    DbSet<TennisMatch> TennisMatches { get; }
    DbSet<MatchMember> MatchMembers { get; }
    DbSet<MatchReview> MatchReviews { get; }
    DbSet<MatchJoinRequest> MatchJoinRequests { get; }
    DbSet<TemporaryMatchScore> TemporaryMatchScores { get; }
    DbSet<PurchasedMatch> PurchasedMatches { get; }

    /// <summary>
    /// Chat
    /// </summary>

    DbSet<ChatHead> ChatHeads { get; }
    DbSet<ChatMember> ChatMembers { get; }
    DbSet<ChatConversation> ChatConversations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
