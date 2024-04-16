using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetMatchCalculationScoreQuery(int matchId) : IRequest<ScoreCalculationContract>;
internal sealed class GetMatchCalculationScoreQueryHandler : IRequestHandler<GetMatchCalculationScoreQuery, ScoreCalculationContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetMatchCalculationScoreQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser,
       IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }
    public async Task<ScoreCalculationContract> Handle(GetMatchCalculationScoreQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var match = _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");
      
        if(match.MatchMembers.Where(x=>x.MemberId == userId).Count() < 1)
            throw new NotFoundException("Only participants can view the score!");

        var ownerTeam = match.MatchMembers.Where(x => x.MemberId == x.CreatedBy).FirstOrDefault();
        var otherTeam = match.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).FirstOrDefault();

        List<int?> teamAScore = new List<int?>();
        List<int?> teamBScore = new List<int?>();

        teamAScore.Add(ownerTeam.SetOnePoint);
        teamAScore.Add(ownerTeam.SetTwoPoint);
        teamAScore.Add(ownerTeam.SetThreePoint);

        teamBScore.Add(otherTeam.SetOnePoint);
        teamBScore.Add(otherTeam.SetTwoPoint);
        teamBScore.Add(otherTeam.SetThreePoint);

        return await _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).Where(x => x.Id == request.matchId).Select(e => new ScoreCalculationContract
        {
            matchId = e.Id,
            teamA = new TeamScoreContract
            {
                teamCode = ownerTeam.TeamCode,
                isMatchWon = ownerTeam.IsMatchWon,
                members = e.MatchMembers.Where(x => x.TeamCode == ownerTeam.TeamCode).Select(x => new TeamUserContract
                {
                    isRated = e.MatchReviews.Where(y => y.CreatedBy == userId && y.ReviewedTo == x.MemberId).Any(),
                    user = _identityService.GetUserDetail(x.MemberId),
                    previousPoint = Math.Round((double)x.PreviousPoint),
                    addMinusPoint = Math.Round((double)x.GamePoint),
                    newPoint = Math.Round((double)x.NewPoints),
                
                }).ToList(),
                scoreList = teamAScore,
            },
            teamB = new TeamScoreContract
            {
                teamCode = otherTeam.TeamCode,
                isMatchWon = otherTeam.IsMatchWon,
                members = e.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).Select(x => new TeamUserContract
                {
                    isRated = e.MatchReviews.Where(y => y.CreatedBy == userId && y.ReviewedTo == x.MemberId).Any(),
                    user = _identityService.GetUserDetail(x.MemberId),
                    previousPoint = Math.Round((double)x.PreviousPoint),
                    addMinusPoint = Math.Round((double)x.GamePoint),
                    newPoint = Math.Round((double)x.NewPoints),
                    
                }).ToList(),
                scoreList = teamBScore,
            },
        }).FirstOrDefaultAsync(cancellationToken) ?? new ScoreCalculationContract();
    }
}

