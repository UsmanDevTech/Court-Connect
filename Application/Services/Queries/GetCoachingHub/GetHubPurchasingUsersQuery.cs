using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;

namespace Application.Services.Queries;


public sealed record GetHubPurchasingUsersQuery(int hubId) : IRequest<List<CouchingHubPurchasedHistory>>;
internal sealed class GetHubPurchasingUsersQueryHandler : IRequestHandler<GetHubPurchasingUsersQuery, List<CouchingHubPurchasedHistory>>
{
    private readonly IIdentityService _identityService;
    public GetHubPurchasingUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<List<CouchingHubPurchasedHistory>> Handle(GetHubPurchasingUsersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetPurchasingUserDetailAsync(request, cancellationToken);
    }
}