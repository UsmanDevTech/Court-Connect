using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Api.Controllers.V1;

[Route("webhook")]

public class WebHooksController : Controller
{
    private ApplicationDbContext _context;
    public WebHooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Obsolete]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);

            // Handle the event
            if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as Invoice;
                var reason = paymentIntent.BillingReason;
                var subscriptionId = paymentIntent.SubscriptionId;

               
                if (reason == "subscription_cycle")
                {
                    

                }
                _context.SaveChanges();
            }

            else if (stripeEvent.Type == Events.InvoicePaymentFailed)
            {
                _context.SaveChanges();
            }
            // ... handle other event types
            else
            {
                // Unexpected event type
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }
}
