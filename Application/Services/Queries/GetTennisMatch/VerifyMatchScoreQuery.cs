using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record VerifyMatchScoreQuery(int matchId, string winnerTeam) : IRequest<Result>;
internal sealed class VerifyMatchScoreQueryHandler : IRequestHandler<VerifyMatchScoreQuery, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IMatchService _matchService;
    public VerifyMatchScoreQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IIdentityService identityService, IMatchService matchService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _matchService = matchService;
    }

    public async Task<Result> Handle(VerifyMatchScoreQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var match = _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.TemporaryMatchScores).Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found!");

        var matchMembers = _context.MatchMembers.Where(x => x.TennisMatchId == request.matchId).ToList();
        if (matchMembers != null && matchMembers.Count() > 0)
        {
            if (matchMembers.Where(x => x.IsScoreApproved == StatusEnum.Rejected).Count() > 0)
            {
                IDictionary<string, string[]> Errors = new Dictionary<string, string[]>();
                Errors.Add("RejectUser", matchMembers.Where(x => x.IsScoreApproved == StatusEnum.Rejected).Select(x => _identityService.GetUserName(x.MemberId)).ToArray());

                throw new ValidationException(Errors);
            }
            else
            {
                //Get owner score
                var ownerTeamCode = matchMembers.Where(x => x.MemberId == x.CreatedBy).Select(x => x.TeamCode).FirstOrDefault();
                var tempMatchScore = match.TemporaryMatchScores.FirstOrDefault();

                foreach (var item in matchMembers)
                {
                    if (item.TeamCode == ownerTeamCode)
                    {
                        item.SetOnePoint = tempMatchScore.TeamAScoreOne;
                        item.SetTwoPoint = tempMatchScore.TeamAScoreTwo;
                        item.SetThreePoint = tempMatchScore.TeamAScoreThree;

                        _context.MatchMembers.Update(item);
                    }
                    else
                    {
                        item.SetOnePoint = tempMatchScore.TeamBScoreOne;
                        item.SetTwoPoint = tempMatchScore.TeamBScoreTwo;
                        item.SetThreePoint = tempMatchScore.TeamBScoreThree;

                        _context.MatchMembers.Update(item);
                    }

                }

                // Score Points Calculation System Start

                var checkOwner = match.MatchMembers.Where(x => x.CreatedBy == userId).FirstOrDefault();

                var player_a_elo = 0.0;
                var player_b_elo = 0.0;

                var games_played_player_a = 0;
                var games_played_player_b = 0;

                List<string> PlayerA = new();
                List<string> PlayerB = new();

                foreach (var item in match.MatchMembers.Where(x => x.TeamCode == checkOwner.TeamCode).ToList())
                {
                    player_a_elo += _identityService.GetUserPoints(item.MemberId);
                    PlayerA.Add(item.MemberId);
                }

                foreach (var item in match.MatchMembers.Where(x => x.TeamCode != checkOwner.TeamCode).ToList())
                {
                    player_b_elo += _identityService.GetUserPoints(item.MemberId);
                    PlayerB.Add(item.MemberId);
                }

                var PlayerAList = _context.MatchMembers.Include(x => x.TennisMatch).AsEnumerable()
                    .Where(x => PlayerA.Contains(x.MemberId) && (x.TennisMatch.Status == MatchStatusEnum.Completed
                    || x.TennisMatch.Status == MatchStatusEnum.Rated)).ToList() ?? new();

                var PlayerBList = _context.MatchMembers.Include(x => x.TennisMatch).AsEnumerable()
                    .Where(x => PlayerB.Contains(x.MemberId) && (x.TennisMatch.Status == MatchStatusEnum.Completed
                    || x.TennisMatch.Status == MatchStatusEnum.Rated)).ToList() ?? new();


                if (PlayerAList.Count() > 0)
                   games_played_player_a = PlayerAList.Count();
                

                if (PlayerBList.Count() > 0)
                    games_played_player_b = PlayerBList.Count();


                if (match.Type != MatchTypeEnum.Single)
                {
                    player_a_elo = player_a_elo / 2;
                    player_b_elo = player_b_elo / 2;
                    games_played_player_a = games_played_player_a / 2;
                    games_played_player_b = games_played_player_b / 2;
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
                    var newPoint = isWon == true ? previousPoint + earnedPoint : previousPoint <= earnedPoint ? 0 : previousPoint - earnedPoint;

                    item.PreviousPoint = previousPoint;
                    item.GamePoint = earnedPoint;
                    item.NewPoints = newPoint;
                    item.updateIsMatchWon(isWon);

                    if (match.MatchCategory == GameTypeEnum.Ranked)
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

                    if (match.MatchCategory == GameTypeEnum.Ranked)
                    {
                        await _identityService.UpdateUserPointAsync(item1.MemberId, newPoint);
                    }

                    _context.MatchMembers.Update(item1);
                }

                await _context.SaveChangesAsync(cancellationToken);

                // Score Points Calculation System End

                var pendingStatus = match.MatchMembers.Where(x => x.IsScoreApproved == StatusEnum.Pending).ToList();
                if (pendingStatus != null && pendingStatus.Count() > 0)
                {
                    foreach (var item in pendingStatus)
                    {
                        item.updateIsScored(StatusEnum.Closed);
                        _context.MatchMembers.Update(item);
                    }
                }

                match.RemoveTempScore(tempMatchScore);

            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}