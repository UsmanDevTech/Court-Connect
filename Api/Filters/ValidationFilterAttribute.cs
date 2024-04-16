using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Domain.Contracts;

namespace Api.Filters;
public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorList = context.ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
            context.Result = new UnprocessableEntityObjectResult(GlobalErrorContract.ReturnError(StatusCodes.Status422UnprocessableEntity, "Invalid ModelState", "Please see attached errors for more information.", "#", errorList));
        }
    }
    public void OnActionExecuted(ActionExecutedContext context) { }
}
