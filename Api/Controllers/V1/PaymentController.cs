using Application.Common.Models;
using Application.Subscriptions.Commands;
using Application.Subscriptions.Queries;
using Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Api.Controllers.V1;

[ApiVersion("1.0")]

public class PaymentController : BaseController
{
    private readonly IHttpContextAccessor _accessor;
    public PaymentController(IHttpContextAccessor accessor)
    {
        StripeConfiguration.ApiKey = "sk_test_51KCfDNBbs68zCZcQT8N6c0cyr3kWFYDv0eL4UNtSObRYNGyUt1upCxRt7O6uTc9eXSk0hSEeA2w9i3CAnAlJspPF00fOXf8oIi";
        _accessor = accessor;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public string BaseUrl()
    {
        var request = _accessor.HttpContext.Request;
        // Now that you have the request you can select what you need from it.
        var url =  "https://" + request.Host.Value + "/";
        return url;
    }

    //Subscription Payment Start
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.pay_by_stripe_subscription + "/{subscriptionId}/{userId}")]
    public async Task<IActionResult> payByStripeSubscription(int subscriptionId, string userId, CancellationToken token)
    {
        var successUrl = "api/v1/Payment/" + Routes.MobilePayment.stripe_subscription_success_url;
        var cancelUrl = "api/v1/Payment/" + Routes.MobilePayment.cancel;
        var failedUrl = "api/v1/Payment/" + Routes.MobilePayment.failed;
        var baseUrl = BaseUrl();

        var url = await Mediator.Send(new PayByStripeSubscriptionQuery(subscriptionId, userId, baseUrl, successUrl, cancelUrl, failedUrl), token);
        return Redirect(url);
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.stripe_subscription_success_url + "/{sessionId}")]
    public async Task<IActionResult> stripeSubscriptionSuccessUrlAsync(string? sessionId, CancellationToken token)
    {
        var successUrl = "api/v1/Payment/" + Routes.MobilePayment.success;
        var baseUrl = BaseUrl();
        var url = await Mediator.Send(new StripeSubscriptionSuccessUrlQuery(sessionId, baseUrl, successUrl), token);
        return Redirect(url);
    }

    //Subscription Payment End

    //Onetime Payment Start
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.one_time_payment + "/{amount}/{userId}")]
    public async Task<IActionResult> payByStripe(double amount, string userId, CancellationToken token)
    {

        var successUrl = "api/v1/Payment/" + Routes.MobilePayment.one_time_payment_success_url;
        var cancelUrl = "api/v1/Payment/" + Routes.MobilePayment.cancel;
        var failedUrl = "api/v1/Payment/" + Routes.MobilePayment.failed;
        var baseUrl = BaseUrl();


        var url = await Mediator.Send(new OneTimeStripePaymentQuery(amount, userId, baseUrl, successUrl, cancelUrl, failedUrl), token);
        return Redirect(url);
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.one_time_payment_success_url + "/{sessionId}")]
    public async Task<IActionResult> success(string sessionId, CancellationToken token)
    {
        var successUrl = "api/v1/Payment/" + Routes.MobilePayment.success;
        var baseUrl = BaseUrl();
        var url = await Mediator.Send(new OneTimeStripePaymentSuccessUrlQuery(sessionId, baseUrl, successUrl), token);
        return Redirect(url);
    }

    //Onetime Payment End

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.success)]
    public IActionResult paymentsuccess(string? sessionId, string paymentIntentId)
    {
        return View();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.cancel)]
    public IActionResult cancel(string sessionId)
    {
        return View();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet(Routes.MobilePayment.failed)]
    public IActionResult failed(string msg)
    {
        ViewBag.error = msg;
        return View();
    }

    //Subscription

    [HttpPost(Routes.Service.purchase_subscription)]
    public async Task<Result> purchaseSubscriptionAsync([FromBody] PurchaseSubscriptionCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Service.unsubscribe_subscription)]
    public async Task<Result> unsubscribeSubscriptionAsync([FromBody] UnsubscribeSubscriptionCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }


    [HttpGet(Routes.Service.get_subscription)]
    public async Task<List<SubscriptionPckContract>> getSubscriptionInformationAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetSubscriptionPackageQuery(), token);
    }

}
