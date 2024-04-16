using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Subscriptions.Commands;

public sealed class PurchaseSubscriptionCommand : IRequest<Result>
{
    public int packageId { get; set; }
    public string paymentId { get; set; } = null!;
    public double amountPaid { get; set; }
}
internal sealed class PurchaseSubscriptionCommandHandler : IRequestHandler<PurchaseSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public PurchaseSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(PurchaseSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        //Purchasing package
        var purchasingPackage = _context.Subscriptions.Where(x => x.Id == request.packageId).FirstOrDefault();
        if (purchasingPackage == null)
            throw new NotFoundException("Package not found");

        var PreviousPackages = _context.SubscriptionHistories.Include(x => x.Subscription).Where(u => u.CreatedBy == userId && u.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel && u.SubscriptionStatus == UserPackageStatusEnum.Active).ToList();
        if (PreviousPackages != null && PreviousPackages.Count > 0)
        {
            foreach (var item in PreviousPackages)
            {
                item.UpdateStatus(UserPackageStatusEnum.TempActive);
                _context.SubscriptionHistories.Update(item);
            }
        }

        //Set Package Expiry
        var expiry = purchasingPackage.DurationType == DurationTypeEnum.Month ? DateTime.UtcNow.AddMonths(1) :
                  purchasingPackage.DurationType == DurationTypeEnum.Day ? DateTime.UtcNow.AddDays(1) :
                  purchasingPackage.DurationType == DurationTypeEnum.Year ? DateTime.UtcNow.AddYears(1)
                  : purchasingPackage.DurationType == DurationTypeEnum.Week ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow;

        expiry = new DateTime(expiry.Year, expiry.Month, expiry.Day, 23, 59, 59, 999);

        //Tax Paid Calculation
        var tax = request.amountPaid - purchasingPackage.PriceAfterDiscount;

        _context.SubscriptionHistories.Add(Domain.Entities.SubscriptionHistory.Create(userId, request.packageId,
            (double)purchasingPackage.Price, (double)purchasingPackage.PriceAfterDiscount, purchasingPackage.IsDiscountAvailable, purchasingPackage.Discount,
            purchasingPackage.CostPerRankedGame, purchasingPackage.CostPerUnrankedGame,
            purchasingPackage.IsFreeRankedUnlimited, purchasingPackage.IsFreeUnrankedUnlimited,
            purchasingPackage.FreeRankedGames, purchasingPackage.FreeUnrankedGames,
            purchasingPackage.FreeRankedGames, purchasingPackage.FreeUnrankedGames,
            purchasingPackage.IsPaidCouchingContentAvailable, purchasingPackage.IsFreeCouchingContentAvailable,
            purchasingPackage.IsRatingAvailable, purchasingPackage.IsMatchBalanceAvailable, purchasingPackage.IsScoreAvailable, purchasingPackage.IsReviewsAvailable,
            expiry, (double)tax, null, request.paymentId, UserPackageStatusEnum.Active, _dateTime.NowUTC));

        await _identityService.UpdateUserPackageDetailAsync(userId);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public class PurchaseSubscriptionCommandValidator : AbstractValidator<PurchaseSubscriptionCommand>
    {
        public PurchaseSubscriptionCommandValidator()
        {
            //Add Model Validation Using Fluent Validation
            RuleFor(u => u.packageId)
                  .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Package Id"));

            RuleFor(u => u.paymentId)
                .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Payment Id"));

            RuleFor(u => u.amountPaid)
              .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Amount"));
        }
    }

}