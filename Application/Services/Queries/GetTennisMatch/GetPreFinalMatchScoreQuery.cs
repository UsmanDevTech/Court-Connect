using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Services.Queries;

public sealed record GetPreFinalMatchScoreQuery(int matchId) : IRequest<ScoreConfirmationContract>;
internal sealed class GetPreFinalMatchScoreQueryHandler : IRequestHandler<GetPreFinalMatchScoreQuery, ScoreConfirmationContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetPreFinalMatchScoreQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<ScoreConfirmationContract> Handle(GetPreFinalMatchScoreQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);
    

        var ownerTeam = _context.MatchMembers.Where(x => x.MemberId == x.CreatedBy && x.TennisMatchId == request.matchId).FirstOrDefault();
        var tempScore = _context.TemporaryMatchScores.Where(x => x.TennisMatchId == request.matchId).FirstOrDefault();

        List<int?> teamAScore = new List<int?>();
        List<int?> teamBScore = new List<int?>();

        teamAScore.Add(tempScore.TeamAScoreOne);
        teamAScore.Add(tempScore.TeamAScoreTwo);
        teamAScore.Add(tempScore.TeamAScoreThree);

        teamBScore.Add(tempScore.TeamBScoreOne);
        teamBScore.Add(tempScore.TeamBScoreTwo);
        teamBScore.Add(tempScore.TeamBScoreThree);

        return await _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchId).Select(e => new ScoreConfirmationContract
        {
            id = e.Id,
            teamA = e.MatchMembers.Where(x => x.TeamCode == ownerTeam.TeamCode).Select(x => new TeamContract
            {
                teamCode = x.TeamCode,
                member = _identityService.GetUserDetail(x.MemberId),
            }).ToList(),
            teamB = e.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).Select(x => new TeamContract
            {
                teamCode = x.TeamCode,
                member = _identityService.GetUserDetail(x.MemberId),
            }).ToList(),
            matchCategory = e.MatchCategory,
            matchType = e.Type,
            title = e.Title,
            TeamAScore = teamAScore,
            TeamBScore = teamBScore,
        }).FirstOrDefaultAsync(cancellationToken) ?? new ScoreConfirmationContract();
    }
}