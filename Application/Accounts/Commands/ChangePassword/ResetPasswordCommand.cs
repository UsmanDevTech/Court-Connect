using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record ResetPasswordCommand(string password) : IRequest<Result>;
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identity;

    public ResetPasswordCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _identity.ResetPasswordAsync(request.password);
    }
}