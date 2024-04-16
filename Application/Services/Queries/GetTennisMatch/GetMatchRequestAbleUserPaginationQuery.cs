using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using MediatR;

namespace Application.Services.Queries;

public sealed record GetMatchRequestAbleUserPaginationQuery(int matchId) : IRequest<List<MatchRequestUsersContract>>;
internal sealed class GetMatchRequestAbleUserPaginationQueryHandler : IRequestHandler<GetMatchRequestAbleUserPaginationQuery, List<MatchRequestUsersContract>>
{
    private readonly IIdentityService _identityService;
    public GetMatchRequestAbleUserPaginationQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<List<MatchRequestUsersContract>> Handle(GetMatchRequestAbleUserPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetMatchRequestAbleUsers(request, cancellationToken);
    }
}