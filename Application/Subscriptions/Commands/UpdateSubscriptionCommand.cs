using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Subscriptions.Commands;

public sealed class UpdateSubscriptionCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string title { get; set; } = null!;
    public double? price { get; set; }
    public double? priceAfterDiscount { get; set; }
    public bool isDiscountAvailable { get; set; }
    public double? discount { get; set; }

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

    public string? priceId { get; set; }

}
internal sealed class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    public UpdateSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var package = _context.Subscriptions.Where(s => s.Id == request.Id).FirstOrDefault();
        if (package == null)
            throw new NotFoundException("Package Not Found");

        package.SetSubscription(request.title, request.price, request.priceAfterDiscount,
       request.isDiscountAvailable, request.discount,
       request.costPerRankedGame, request.costPerUnrankedGame,
       request.isFreeRankedGameUnlimited, request.isFreeUnrankedGameUnlimited,
       request.freeRankedGames, request.freeUnrankedGames,
       request.isPaidCouchingContentAvailable, request.isFreeCouchingContentAvailable,
       request.isRatingAvailable, request.isMatchBalanceAvailable, request.isScoreAvailable, request.isReviewAvailable,
       request.priceId);

        _context.Subscriptions.Update(package);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
