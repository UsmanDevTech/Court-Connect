using MediatR;
using Stripe.Checkout;

namespace Application.Subscriptions.Queries;

public sealed record OneTimeStripePaymentSuccessUrlQuery(string? sessionId, string baseUrl, string success) : IRequest<string>;
internal sealed class OneTimeStripePaymentSuccessUrlQueryHandler : IRequestHandler<OneTimeStripePaymentSuccessUrlQuery, string>
{
    public async Task<string> Handle(OneTimeStripePaymentSuccessUrlQuery request, CancellationToken cancellationToken)
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
            url = request.baseUrl + request.success + "?sessionId=" + request.sessionId + "&paymentIntentId=" + session.PaymentIntentId;
        }

        return url;
    }
}