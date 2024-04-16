using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enum;
using FluentValidation;
using MediatR;
using Stripe;

namespace Application.Subscriptions.Commands;

public sealed record UnsubscribeSubscriptionCommand(int subscribedId) : IRequest<Result>;

internal sealed class UnsubscribeSubscriptionCommandHandler : IRequestHandler<UnsubscribeSubscriptionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public UnsubscribeSubscriptionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(UnsubscribeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        //Subscribed subscription
        var purchasingPackage = _context.SubscriptionHistories.Where(x => x.Id == request.subscribedId).FirstOrDefault();
        if (purchasingPackage == null)
            throw new NotFoundException("Subscription not found");


        //var stripeSecretkey = _context.AppInfos.Where(u => u.Type == ContentType.StripeKey).Select(u => u.Content).FirstOrDefault();
        StripeConfiguration.ApiKey = "sk_test_51KCfDNBbs68zCZcQT8N6c0cyr3kWFYDv0eL4UNtSObRYNGyUt1upCxRt7O6uTc9eXSk0hSEeA2w9i3CAnAlJspPF00fOXf8oIi";

        var service = new SubscriptionService();
        var cancelOptions = new SubscriptionCancelOptions
        {
            InvoiceNow = false,
            Prorate = false,
        };
        Subscription subscription = service.Cancel(purchasingPackage.PaymentId, cancelOptions);
        //Cancel Subscription End

        purchasingPackage.UpdateStatus(UserPackageStatusEnum.Unsubscribed);

        _context.SubscriptionHistories.Update(purchasingPackage);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public class UnsubscribeSubscriptionCommandValidator : AbstractValidator<UnsubscribeSubscriptionCommand>
    {
        public UnsubscribeSubscriptionCommandValidator()
        {
            //Add Model Validation Using Fluent Validation
            RuleFor(u => u.subscribedId)
                  .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Package Id"));
        }
    }

}