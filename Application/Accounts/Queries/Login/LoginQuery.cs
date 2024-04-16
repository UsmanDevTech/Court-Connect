using Application.Common.Interfaces;
using Domain.Common;
using Domain.Contracts;
using FluentValidation;
using MediatR;
namespace Application.Accounts.Queries;
public sealed record LoginQuery(string email,string password,string? fcmToken, string? timeZoneId) : IRequest<ResponseKeyContract>;
internal sealed class LoginQueryHandler : IRequestHandler<LoginQuery, ResponseKeyContract>
{
    private readonly IIdentityService _users;
    public LoginQueryHandler(IIdentityService users)
    {
        _users = users;
    }
    public async Task<ResponseKeyContract> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        return await _users.AuthenticateUserAsync(request);
    }
}
public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(v => v.email)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Email"))
            .EmailAddress().WithMessage(InvalidOperationErrorMessage.InvalidEmailFormat("{PropertyValue}"));

        RuleFor(v => v.password)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Password"));
    }
}
