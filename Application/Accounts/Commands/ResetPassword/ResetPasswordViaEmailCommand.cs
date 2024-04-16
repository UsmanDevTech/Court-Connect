using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enum;
using FluentValidation;
using MediatR;
namespace Application.Accounts.Commands;
public sealed record ResetPasswordViaEmailCommand(string email,int? resetOption, string resetValue, string password): IRequest<Result>;
internal sealed class ResetPasswordViaEmailCommandHandler : IRequestHandler<ResetPasswordViaEmailCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordViaEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ResetPasswordViaEmailCommand request, CancellationToken cancellationToken)
    {
        var response = await _identityService.ResetPasswordViaEmailAsync(request);
        return response.Result;
    }
}
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordViaEmailCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(u => u.resetOption)
            .Must(IsResetOptionValid).WithMessage(InvalidOperationErrorMessage.MustInBetweenErrorMessage("Reset Option", "1-0, 0 => with otp, 1 => with password"));

        RuleFor(u => u.email)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Email"))
            .EmailAddress().WithMessage(InvalidOperationErrorMessage.InvalidEmailFormat(""));

        RuleFor(u => u.resetValue)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Reset Value"));

        RuleFor(u => u.password)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Password"));
    }
    private bool IsResetOptionValid(int? index)
    {
        if (index == null)
            return false;

        ResetPasswordOptionEnum enumVal = (ResetPasswordOptionEnum)Enum.Parse(typeof(ResetPasswordOptionEnum), index.ToString());

        if (!Enum.IsDefined(typeof(ResetPasswordOptionEnum), enumVal))
            return false;

        return true;
    }
}