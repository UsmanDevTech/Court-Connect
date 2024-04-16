using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;

namespace Application.Subscriptions.Commands;

public sealed class AddSubscriptionCommand : IRequest<Result>
{
    public string title { get; set; } = null!;
    public double? price { get; set; }
    public double? priceAfterDiscount { get; set; }
    public bool isDiscountAvailable { get; set; }
    public double? discount { get; set; }
    public SubscriptionTypeEnum subscriptionType { get; set; }
    public DurationTypeEnum durationType { get; set; }

    public bool isFreeRankedGameUnlimited { get; set; }
    public bool isFreeUnrankedGameUnlimited { get; set; }
    public int freeRankedGames { get; set; }
    public int freeUnrankedGames { get; set; }

    public double? costPerRankedGame { get; set; }
    public double? costPerUnrankedGame { get; set; }

    public bool isFreeCouchingContentAvailable { get; set; }
    public bool isPaidCouchingContentAvailable { get; set; }

    public bool isRatingAvailable { get; set; }
    public bool isMatchBalanceAvailable { get; set; }
    public bool isScoreAvailable { get; set; }
    public bool isReviewAvailable { get; set; }

    public string? productId { get; set; }
    public string? priceId { get; set; }

}
internal sealed class AddSubscriptionCommandHandler : IRequestHandler<AddSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    public AddSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(AddSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var package = Domain.Entities.Subscription.Create(request.title, request.price, request.priceAfterDiscount,
       request.isDiscountAvailable, request.discount, request.subscriptionType, request.durationType,
       request.costPerRankedGame, request.costPerUnrankedGame,
       request.isFreeRankedGameUnlimited, request.isFreeUnrankedGameUnlimited,
       request.freeRankedGames, request.freeUnrankedGames,
       request.isPaidCouchingContentAvailable, request.isFreeCouchingContentAvailable,
       request.isRatingAvailable, request.isMatchBalanceAvailable, request.isScoreAvailable, request.isReviewAvailable, request.productId,
       request.priceId, _currentUser.UserId, _dateTime.NowUTC);
        _context.Subscriptions.Add(package);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}