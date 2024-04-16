using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Accounts.Commands;

public sealed record SendEmailOtpCommand(string existingEmail, string newEmail) : IRequest<Result>;

internal sealed class SendEmailOtpCommandHandler : IRequestHandler<SendEmailOtpCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SendEmailOtpCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(SendEmailOtpCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SendEmailOTPAsync(request, cancellationToken);
    }
}
public class SendOtpCommandValidator : AbstractValidator<SendEmailOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(u => u.existingEmail)
            .EmailAddress().WithMessage(InvalidOperationErrorMessage.InvalidEmailFormat(""))
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Current"));
        RuleFor(u => u.newEmail)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Replacement"));

    }
}