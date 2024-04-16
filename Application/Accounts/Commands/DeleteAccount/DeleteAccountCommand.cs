using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record DeleteAccountCommand(string password) : IRequest<Result>;
public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
{
    private readonly IIdentityService _identity;

    public DeleteAccountCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        return await _identity.RequestForAccountDeleteAsync(request.password);
    }
}
