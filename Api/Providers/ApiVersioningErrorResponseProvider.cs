using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Domain.Contracts;

namespace Api.Providers;
public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
{
    // note: in Web API the response type is HttpResponseMessage
    public override IActionResult CreateResponse(ErrorResponseContext context)
    {
        //You can initialize your own class here. Below is just a sample.
        var response = new ObjectResult(GlobalErrorContract.ReturnError(context.StatusCode, context.ErrorCode, context.MessageDetail ?? context.Message, context.Request?.Path.Value, null));
        response.StatusCode = (int)HttpStatusCode.BadRequest;

        return response;
    }
}
