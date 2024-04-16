using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Subscriptions.Queries;

public sealed record GetSubscriptionPackageQuery : IRequest<List<SubscriptionPckContract>>;
internal sealed class GetSubscriptionPackageQueryHandler : IRequestHandler<GetSubscriptionPackageQuery, List<SubscriptionPckContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetSubscriptionPackageQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<List<SubscriptionPckContract>> Handle(GetSubscriptionPackageQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return await _context.Subscriptions.Include(x => x.SubscriptionHeadings).Where(x => x.Deleted == false).OrderByDescending(x => x.Created).Select(u => new SubscriptionPckContract
        {
            id = u.Id,
            title = u.Title,
            price = u.Price,
            priceAfterDiscount = u.PriceAfterDiscount,
            discount = u.Discount,
            isDiscountAvailable = u.IsDiscountAvailable,
            durationType = u.DurationType,
            subscriptionType = u.SubscriptionType,

            isFreeCouchingContentAvailable = u.IsFreeCouchingContentAvailable,
            isPaidCouchingContentAvailable = u.IsPaidCouchingContentAvailable,
            costPerRankedGame = u.CostPerRankedGame,
            costPerUnrankedGame = u.CostPerUnrankedGame,

            isFreeRankedGameUnlimited = u.IsFreeRankedUnlimited,
            isFreeUnrankedGameUnlimited = u.IsFreeUnrankedUnlimited,
            freeRankedGames = u.FreeRankedGames,
            freeUnrankedGames = u.FreeUnrankedGames,

            isScoreAvailable = u.IsScoreAvailable,
            isReviewAvailable = u.IsReviewsAvailable,
            isMatchBalanceAvailable = u.IsMatchBalanceAvailable,
            isRatingAvailable = u.IsRatingAvailable,
            headings = u.SubscriptionHeadings.Select(x => new SubscriptionPointDtlsContract
            {
                Title = x.Title,
                points = _context.SubscriptionPoints.Where(a => a.SubscriptionHeadingId == x.Id).Select(y => new GenericIconInfoContract
                {
                    id = y.Id,
                    iconUrl = y.Icon ?? "",
                    name = y.Detail ?? "",
                }).ToList(),
            }).ToList(),
        }).ToListAsync(cancellationToken);
    }
}