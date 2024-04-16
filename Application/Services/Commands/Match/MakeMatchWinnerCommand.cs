using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record MakeMatchWinnerCommand(int matchId, string winnerTeam) : IRequest<Result>;
internal sealed class MakeMatchWinnerCommandHandler : IRequestHandler<MakeMatchWinnerCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMatchService _matchService;
    private readonly IIdentityService _identityService;
   
    public MakeMatchWinnerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IMatchService matchService, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _matchService = matchService;
        _identityService = identityService;
    }
    public async Task<Result> Handle(MakeMatchWinnerCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var match = _context.TennisMatches.Include(x => x.MatchJoinRequests).Include(x => x.MatchMembers)
            .Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");

        var checkOwner = match.MatchMembers.Where(x => x.CreatedBy == userId).FirstOrDefault();
        if (checkOwner == null)
            throw new CustomInvalidOperationException("Only owner can make winner!");

        var player_a_elo = 0.0;
        var player_b_elo = 0.0;

        foreach (var item in match.MatchMembers.Where(x => x.TeamCode == checkOwner.TeamCode).ToList())
        {
            player_a_elo += _identityService.GetUserPoints(item.MemberId);
        }

        foreach (var item in match.MatchMembers.Where(x => x.TeamCode != checkOwner.TeamCode).ToList())
        {
            player_b_elo += _identityService.GetUserPoints(item.MemberId);
        }

        if (match.Type != MatchTypeEnum.Single)
        {
            player_a_elo = player_a_elo / 2;
            player_b_elo = player_b_elo / 2;
        }

        match.updateMatchStatus(MatchStatusEnum.Completed);
        _context.TennisMatches.Update(match);

        // Calculate earned point
        List<string> score = new List<string>();
        var tie_break = "";

        var otherTeamMember = match.MatchMembers.Where(x => x.TeamCode != checkOwner.TeamCode).OrderBy(x => x.Created).FirstOrDefault();

        score.Add(checkOwner.SetOnePoint.ToString() + ":" + otherTeamMember.SetOnePoint.ToString());
        score.Add(checkOwner.SetTwoPoint.ToString() + ":" + otherTeamMember.SetTwoPoint.ToString());

        if (checkOwner.SetThreePoint != 0 && otherTeamMember.SetThreePoint != 0)
            tie_break = checkOwner.SetThreePoint.ToString() + ":" + otherTeamMember.SetThreePoint.ToString();

        //Calculation formula for the score
        var games_played_player_a = 5;
        var games_played_player_b = 5;

        var earnedScores = _matchService.CalculateNewEloRating(player_a_elo, player_b_elo, score, tie_break,
            games_played_player_a, games_played_player_b);

        string[] earnedScoreList = earnedScores.Split(":");

        var userAEarnedPoint = Convert.ToDouble(earnedScoreList[0]);
        var userBEarnedPoint = Convert.ToDouble(earnedScoreList[1]);

        foreach (var item in match.MatchMembers.Where(x => x.TeamCode == checkOwner.TeamCode).ToList())
        {
            var isWon = request.winnerTeam == "A" ? true : false;
            
            var previousPoint = _identityService.GetUserPoints(item.MemberId);
            var earnedPoint = userAEarnedPoint;
            var newPoint = isWon == true ? previousPoint + earnedPoint : previousPoint <= earnedPoint? 0: previousPoint - earnedPoint;

            item.PreviousPoint = previousPoint;
            item.GamePoint = earnedPoint;
            item.NewPoints = newPoint;
            item.updateIsMatchWon(isWon);

            if(match.MatchCategory == GameTypeEnum.Ranked)
                await _identityService.UpdateUserPointAsync(item.MemberId, newPoint);
         
            _context.MatchMembers.Update(item);
        }

        foreach (var item1 in match.MatchMembers.Where(x => x.TeamCode != checkOwner.TeamCode).ToList())
        {
            var isWon = request.winnerTeam == "B" ? true : false;
           
            var previousPoint = _identityService.GetUserPoints(item1.MemberId);
            var earnedPoint = userBEarnedPoint;
            var newPoint = isWon == true ? previousPoint + earnedPoint : previousPoint <= earnedPoint ? 0 : previousPoint - earnedPoint;

            item1.PreviousPoint = previousPoint;
            item1.GamePoint = earnedPoint;
            item1.NewPoints = newPoint;
            item1.updateIsMatchWon(isWon);

            if (match.MatchCategory == GameTypeEnum.Ranked) { 
               await _identityService.UpdateUserPointAsync(item1.MemberId, newPoint);
            }

            _context.MatchMembers.Update(item1);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

