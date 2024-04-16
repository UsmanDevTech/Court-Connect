using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Subscriptions.Queries;

public sealed record GetSubscriptionByUserIdQuery(string userId) : IRequest<List<SubscriptionDtlsContract>>;
internal sealed class GetSubscriptionByUserIdQueryHandler : IRequestHandler<GetSubscriptionByUserIdQuery, List<SubscriptionDtlsContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTimeService;
    public GetSubscriptionByUserIdQueryHandler(IApplicationDbContext context, IIdentityService identityService,
        IDateTime dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _identityService = identityService;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<List<SubscriptionDtlsContract>> Handle(GetSubscriptionByUserIdQuery request, CancellationToken cancellationToken)
    {
        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);

        return await _context.SubscriptionHistories.Include(x => x.Subscription).Where(x => x.CreatedBy == request.userId).OrderByDescending(x=>x.Created).Select(x => new SubscriptionDtlsContract
        {
            id = x.Id,
            subscriptionId = x.SubscriptionId,
            subscriptionPurchasedStatus = x.SubscriptionStatus,
            subscriptionType = x.Subscription.SubscriptionType,
            isScoreAvailable = x.IsScoreAvailable,
            costPerRankedGame = x.CostPerRankedGame,
            costPerUnrankedGame = x.CostPerUnrankedGame,
            discount = x.Discount,
            durationType = x.Subscription.DurationType,
            freeRankedGames = x.FreeRankedGames,
            freeUnrankedGames = x.FreeUnrankedGames,
            isDiscountAvailable = x.IsDiscountAvailable,
            isFreeCouchingContentAvailable = x.IsFreeCouchingContentAvailable,
            isFreeRankedGameUnlimited = x.IsFreeRankedUnlimited,
            isFreeUnrankedGameUnlimited = x.IsFreeUnrankedUnlimited,
            isMatchBalanceAvailable = x.IsMatchBalanceAvailable,
            isPaidCouchingContentAvailable = x.IsPaidCouchingContentAvailable,
            isRatingAvailable = x.IsRatingAvailable,
            isReviewAvailable = x.IsReviewsAvailable,
            price = x.Price,
            priceAfterDiscount = x.PriceAfterDiscount,
            remainingFreeRankedMatches = x.RemainingFreeRankedGames,
            remainingFreeUnrankedMatches = x.RemainingFreeUnrankedGames,
            title = x.Subscription.Title,
            created = x.Created.UtcToLocalTime(timezoneId).ToString(_dateTimeService.longDayDateTimeFormat)
        }).ToListAsync(cancellationToken);
    }
}