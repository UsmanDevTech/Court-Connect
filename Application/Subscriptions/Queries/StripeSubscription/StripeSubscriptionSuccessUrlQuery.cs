using MediatR;
using Stripe.Checkout;

namespace Application.Subscriptions.Queries;

public sealed record StripeSubscriptionSuccessUrlQuery(string? sessionId, string baseUrl, string success) : IRequest<string>;
internal sealed class StripeSubscriptionSuccessUrlQueryHandler : IRequestHandler<StripeSubscriptionSuccessUrlQuery, string>
{
    public async Task<string> Handle(StripeSubscriptionSuccessUrlQuery request, CancellationToken cancellationToken)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(request.sessionId);

        string url;
        if (session == null)
        {
            url = request.baseUrl + request.success + "?sessionId=" + request.sessionId + "&paymentIntentId=" + null;
        }
        else
        {
            url = request.baseUrl + request.success + "?sessionId=" + request.sessionId + "&paymentIntentId=" + session.SubscriptionId;
        }

        return url;
    }
}