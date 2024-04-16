using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record RemoveProfileImageCommand : IRequest<Result>;
internal sealed class RemoveProfileImageCommandHandler : IRequestHandler<RemoveProfileImageCommand, Result>
{
    private readonly IIdentityService _identityService;

    public RemoveProfileImageCommandHandler(IIdentityService identityService, ICurrentUserService currentUser)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(RemoveProfileImageCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RemoveProfileImageAsync();
    }
}