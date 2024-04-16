using Application.Common.Interfaces;
using MediatR;

namespace Application.Subscriptions.Queries;

public sealed record OneTimeStripePaymentQuery(double amount, string userId, string baseUrl, string success, string cancel, string failed) : IRequest<string>;
internal sealed class OneTimeStripePaymentQueryHandler : IRequestHandler<OneTimeStripePaymentQuery, string>
{
    private readonly IIdentityService _user;

    public OneTimeStripePaymentQueryHandler(IIdentityService user)
    {
        _user = user;
    }

    public async Task<string> Handle(OneTimeStripePaymentQuery request, CancellationToken cancellationToken)
    {
        return await _user.OneTimeStripePaymentAsync(request.amount, request.userId, request.baseUrl, request.success, request.cancel, request.failed, cancellationToken);
    }
}