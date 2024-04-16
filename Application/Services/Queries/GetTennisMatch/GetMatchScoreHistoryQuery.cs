using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetMatchScoreHistoryQuery(int matchId) : IRequest<MatchScoreCalculationContract>;
internal sealed class GetMatchScoreHistoryQueryHandler : IRequestHandler<GetMatchScoreHistoryQuery, MatchScoreCalculationContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetMatchScoreHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser,
       IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }
    public async Task<MatchScoreCalculationContract> Handle(GetMatchScoreHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var match = _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");

        //if (match.MatchMembers.Where(x => x.MemberId == userId).Count() < 1)
        //    throw new NotFoundException("Only participants can view the score!");

        var ownerTeam = match.MatchMembers.Where(x => x.MemberId == x.CreatedBy).FirstOrDefault();
        var otherTeam = match.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).FirstOrDefault();

        List<int?> teamAScore = new List<int?>();
        List<int?> teamBScore = new List<int?>();

        teamAScore.Add(ownerTeam.SetOnePoint);
        teamAScore.Add(ownerTeam.SetTwoPoint);
        teamAScore.Add(ownerTeam.SetThreePoint);
        if(otherTeam != null)
        {
            teamBScore.Add(otherTeam.SetOnePoint);
            teamBScore.Add(otherTeam.SetTwoPoint);
            teamBScore.Add(otherTeam.SetThreePoint);
            return await _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).Where(x => x.Id == request.matchId).Select(e => new MatchScoreCalculationContract
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
                        previousPoint = x.PreviousPoint,
                        addMinusPoint = x.GamePoint,
                        newPoint = x.NewPoints,
                    }).ToList(),
                    scoreList = teamAScore,
                } ?? null,
                teamB = new TeamScoreContract
                {
                    teamCode = otherTeam.TeamCode,
                    isMatchWon = otherTeam.IsMatchWon,
                    members = e.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).Select(x => new TeamUserContract
                    {
                        isRated = e.MatchReviews.Where(y => y.CreatedBy == userId && y.ReviewedTo == x.MemberId).Any(),
                        user = _identityService.GetUserDetail(x.MemberId),
                        previousPoint = x.PreviousPoint,
                        addMinusPoint = x.GamePoint,
                        newPoint = x.NewPoints,
                    }).ToList(),
                    scoreList = teamBScore ?? null,
                } ?? null,
            }).FirstOrDefaultAsync(cancellationToken) ?? new MatchScoreCalculationContract();
        }
        else
        {
            return await _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).Where(x => x.Id == request.matchId).Select(e => new MatchScoreCalculationContract
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
                        previousPoint = x.PreviousPoint,
                        addMinusPoint = x.GamePoint,
                        newPoint = x.NewPoints,
                    }).ToList(),
                    scoreList = teamAScore,
                } ?? null,
                teamB = null,
            }).FirstOrDefaultAsync(cancellationToken) ?? new MatchScoreCalculationContract();
        }

    }
}
