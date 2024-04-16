using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record LogoutCommand() : IRequest<Result>;
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IIdentityService _identity;

    public LogoutCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await _identity.LogoutAsync();
    }
}