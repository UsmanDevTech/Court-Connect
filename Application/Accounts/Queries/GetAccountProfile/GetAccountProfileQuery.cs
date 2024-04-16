using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
namespace Application.Accounts.Queries;
public sealed record GetAccountProfileQuery(string? userId):IRequest<UserProfileInfoDetailContract>;
internal sealed class GetAccountProfileQueryHandler : IRequestHandler<GetAccountProfileQuery, UserProfileInfoDetailContract>
{
    private readonly IIdentityService _identityService;

    public GetAccountProfileQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<UserProfileInfoDetailContract> Handle(GetAccountProfileQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetAccountProfileAsync(request);
    }
}
