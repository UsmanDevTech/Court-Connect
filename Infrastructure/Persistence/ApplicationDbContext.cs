using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.Identity;
using System.Reflection;
using Infrastructure.Persistence.Interceptors;
using MediatR;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }



    /// <summary>
    /// Setting
    /// </summary>
    public DbSet<AppContent> AppContent => Set<AppContent>();
    public DbSet<AccountOtpHistory> AccountOtpHistory => Set<AccountOtpHistory>();

    /// <summary>
    /// League
    /// </summary>
    public DbSet<League> Leagues => Set<League>();
    public DbSet<SubLeague> SubLeagues => Set<SubLeague>();
    public DbSet<LeagueReward> LeagueRewards => Set<LeagueReward>();

    /// <summary>
    /// Application User
    /// </summary>

    public DbSet<UserInterest> UserInterests => Set<UserInterest>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();
    public DbSet<ProfileWarning> ProfileWarnings => Set<ProfileWarning>();


    /// <summary>
    /// Subscriptions
    /// </summary>
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionHeading> SubscriptionHeadings => Set<SubscriptionHeading>();
    public DbSet<SubscriptionPoint> SubscriptionPoints => Set<SubscriptionPoint>();
    public DbSet<SubscriptionHistory> SubscriptionHistories => Set<SubscriptionHistory>();

    /// <summary>
    /// Couching Hub
    /// </summary>
    public DbSet<CouchingHubCategory> CouchingHubCategories => Set<CouchingHubCategory>();
    public DbSet<CouchingHub> CouchingHubs => Set<CouchingHub>();
    public DbSet<CouchingHubDetail> CouchingHubDetails => Set<CouchingHubDetail>();
    public DbSet<CouchingHubBenifit> CouchingHubBenifits => Set<CouchingHubBenifit>();
    public DbSet<PurchasedClasses> PurchasedClassess => Set<PurchasedClasses>();

    /// <summary>
    /// Tennis Match
    /// </summary>
    public DbSet<TennisMatch> TennisMatches => Set<TennisMatch>();
    public DbSet<MatchMember> MatchMembers => Set<MatchMember>();
    public DbSet<MatchReview> MatchReviews => Set<MatchReview>();
    public DbSet<MatchJoinRequest> MatchJoinRequests => Set<MatchJoinRequest>();
    public DbSet<TemporaryMatchScore> TemporaryMatchScores => Set<TemporaryMatchScore>();
    public DbSet<PurchasedMatch> PurchasedMatches => Set<PurchasedMatch>();
    public DbSet<MatchLocation> MatchLocations => Set<MatchLocation>();

    /// <summary>
    /// Notifcation
    /// </summary>
    public DbSet<Notification> Notifications => Set<Notification>();


    /// <summary>
    /// Chat
    /// </summary>
    public DbSet<ChatHead> ChatHeads => Set<ChatHead>();
    public DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    public DbSet<ChatConversation> ChatConversations => Set<ChatConversation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
