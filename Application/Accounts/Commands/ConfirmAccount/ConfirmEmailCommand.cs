using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Contracts;
using FluentValidation;
using MediatR;

namespace Application.Accounts.Commands;
public sealed record ConfirmEmailCommand(string email, string otp) : IRequest<ResponseKeyContract>;
internal sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ResponseKeyContract>
{
    private readonly IIdentityService _identityService;
    public ConfirmEmailCommandHandler(IIdentityService identityService, ICurrentUserService currentUser)
    {
        _identityService = identityService;
    }

    public async Task<ResponseKeyContract> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ConfirmEmailAsync(request, cancellationToken);
    }
}
public class VerifyAccountCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public VerifyAccountCommandValidator()
    {
        RuleFor(u => u.otp)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("One Time Password (OTP)"));

        RuleFor(u => u.email)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Email"));
    }
}
