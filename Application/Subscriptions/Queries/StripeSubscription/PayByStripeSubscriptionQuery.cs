using Application.Common.Interfaces;
using MediatR;
namespace Application.Subscriptions.Queries;

public sealed record PayByStripeSubscriptionQuery(int subscriptionId, string userId, string baseUrl, string success, string cancel, string failed) : IRequest<string>;
internal sealed class PayByStripeSubscriptionQueryHandler : IRequestHandler<PayByStripeSubscriptionQuery, string>
{
    private readonly IIdentityService _user;

    public PayByStripeSubscriptionQueryHandler(IIdentityService user)
    {
        _user = user;
    }

    public async Task<string> Handle(PayByStripeSubscriptionQuery request, CancellationToken cancellationToken)
    {
        return await _user.PayByStripeSubscriptionAsync(request.subscriptionId, request.userId, request.baseUrl, request.success, request.cancel, request.failed, cancellationToken);
    }
}