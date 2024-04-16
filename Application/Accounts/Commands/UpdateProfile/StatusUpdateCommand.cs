using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record StatusUpdateCommand(string id, int isDeleted, string? reason, int loginRole) : IRequest<Result>;
internal sealed class StatusUpdateCommandHandler : IRequestHandler<StatusUpdateCommand, Result>
{
    private readonly IIdentityService _identityService;

    public StatusUpdateCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(StatusUpdateCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdateUserStatusAsync(request);
    }
}
