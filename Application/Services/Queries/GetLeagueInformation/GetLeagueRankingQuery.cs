using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;


namespace Application.Services.Queries;

public sealed record GetLeagueRankingQuery : IRequest<LeagueRankingContract>;
internal sealed class GetLeagueRankingQueryHandler : IRequestHandler<GetLeagueRankingQuery, LeagueRankingContract>
{
    private readonly IIdentityService _identityService;
    public GetLeagueRankingQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LeagueRankingContract> Handle(GetLeagueRankingQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetLeagueRanking(cancellationToken);
    }
}