using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Domain.Contracts;

namespace Api.Filters;
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{

    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    private readonly IHttpContextAccessor _contextAccessor;
    public ApiExceptionFilterAttribute(IHttpContextAccessor contextAccessor)
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
                { typeof(CustomInvalidOperationException), HandleCustomInvalidOperationException },
            };
        _contextAccessor = contextAccessor;
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }

        HandleUnknownException(context);
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;
        
        context.Result = new BadRequestObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status400BadRequest, "Some Validation Failed.", "Please see attached errors for more information.", _contextAccessor.HttpContext?.GetEndpoint()?.ToString(), exception.Errors));
        context.ExceptionHandled = true;
    }

    private void HandleInvalidModelStateException(ExceptionContext context)
    {

        var errorList = context.ModelState.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        );
        context.Result = new BadRequestObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status400BadRequest, "Invalid ModelState", "Please see attached errors for more information.", "#", errorList));

        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException) context.Exception;
        
        context.Result = new NotFoundObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status404NotFound, "The specified resource was not found.", exception.Message, _contextAccessor.HttpContext?.GetEndpoint()?.ToString(), null));

        context.ExceptionHandled = true;
    }
    private void HandleCustomInvalidOperationException(ExceptionContext context)
    {
        var exception = (CustomInvalidOperationException) context.Exception;
        
        context.Result = new UnprocessableEntityObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status422UnprocessableEntity, "Request is not processable.", exception.Message, _contextAccessor.HttpContext?.GetEndpoint()?.ToString(), null));

        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        context.Result = new ObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status401Unauthorized, "Unauthorized", "The server could not verify that you are authorized to access the document requested. Either you supplied the wrong credentials (e.g bad password), or your browser doesn't understand how to supply the credentials required.", "#", null))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        context.Result = new ObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status403Forbidden, "Forbidden", "You don't have permission to access this resource.", "#",null))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        context.Result = new ObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status500InternalServerError, "UnknownException", context?.Exception?.Message??"Exception didn't catch by default. please contact admin for more detail.", "#", null))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
