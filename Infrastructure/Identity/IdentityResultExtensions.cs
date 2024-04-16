using Application.Common.Exceptions;
using Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        if (!result.Succeeded)
        {
            IDictionary<string, string[]> Errors = new Dictionary<string, string[]>();
            Errors.Add("", result.Errors.Select(e => e.Description).ToArray());
            throw new ValidationException(Errors);
        }

        return Result.Success();       
    }
}
