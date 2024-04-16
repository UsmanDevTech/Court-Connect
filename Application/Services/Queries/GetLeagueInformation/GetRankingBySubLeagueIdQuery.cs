using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Queries;

//public sealed record GetRankingBySubLeagueIdQuery(int leagueId) : IRequest<List<LeagueRankingUserContract>>;
//internal sealed class GetRankingBySubLeagueIdQueryHandler : IRequestHandler<GetRankingBySubLeagueIdQuery, List<LeagueRankingUserContract>>
//{
//    private readonly IIdentityService _identityService;
//    public GetRankingBySubLeagueIdQueryHandler(IIdentityService identityService)
//    {
//        _identityService = identityService;
//    }

//    public async Task<List<LeagueRankingUserContract>> Handle(GetRankingBySubLeagueIdQuery request, CancellationToken cancellationToken)
//    {
//        //return await _identityService.GetRankingBySubLeague(cancellationToken);
//    }
//}