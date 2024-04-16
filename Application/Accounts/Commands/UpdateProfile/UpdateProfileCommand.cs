using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;
public sealed record UpdateProfileCommand(string? name, string? phone, string? profileImageUrl,
    string? clubName, string? address, double? latitute, double? longitute, double? radius, 
    List<string>? interests,string? about) : IRequest<Result>;
internal sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdateAccountDetailAsync(request, cancellationToken);
    }
}
