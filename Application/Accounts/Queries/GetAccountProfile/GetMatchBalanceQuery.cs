
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Queries;

public sealed record GetMatchBalanceQuery(string? otherUser) : IRequest<MatchBalanceContract>;
internal sealed class GetMatchBalanceQueryHandler : IRequestHandler<GetMatchBalanceQuery, MatchBalanceContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetMatchBalanceQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<MatchBalanceContract> Handle(GetMatchBalanceQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        DateTime s_date = DateTime.UtcNow.AddMonths(-1);
        DateTime Start_check_Date = new DateTime(s_date.Year, s_date.Month, 1);
        DateTime End_check_Date = new DateTime(s_date.AddMonths(1).Year, s_date.AddMonths(1).Month, 1).AddMinutes(-1);

        var setWon = 0;
        var setLoss = 0;
        var gameWon = 0;
        var gameLoss = 0;
        double? pointGainedLoss = 0.0;

        if (request.otherUser != "" && request.otherUser != null)
        {

            //Point gained or loss calculation
            List<TennisMatch> matches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o => o.MatchDateTime <= End_check_Date && o.MatchDateTime >= Start_check_Date
             && (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(request.otherUser)).ToList();

            if (matches != null && matches.Count() > 0)
            {
                foreach (var item in matches)
                {
                    var matchMember = item.MatchMembers.Where(x => x.MemberId == request.otherUser).FirstOrDefault();
                    if (matchMember.IsMatchWon == true)
                        pointGainedLoss += matchMember.GamePoint;
                    else
                        pointGainedLoss -= matchMember.GamePoint;
                }
            }

            //Set gained or loss calculation
            List<TennisMatch> setMatches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o =>
             (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(request.otherUser)).ToList();

            if (setMatches != null && setMatches.Count() > 0)
            {
                foreach (var item in setMatches)
                {
                    var meAsMember = item.MatchMembers.Where(x => x.MemberId == request.otherUser).FirstOrDefault();
                    if (meAsMember != null)
                    {
                        var otherMember = item.MatchMembers.Where(x => x.TeamCode != meAsMember.TeamCode).FirstOrDefault();

                        if (meAsMember.SetOnePoint > otherMember.SetOnePoint)
                            setWon += 1;
                        if (meAsMember.SetTwoPoint > otherMember.SetTwoPoint)
                            setWon += 1;
                        if (meAsMember.SetThreePoint > otherMember.SetThreePoint)
                            setWon += 1;
                        if (otherMember.SetOnePoint > meAsMember.SetOnePoint)
                            setLoss += 1;
                        if (otherMember.SetTwoPoint > meAsMember.SetTwoPoint)
                            setLoss += 1;
                        if (otherMember.SetThreePoint > meAsMember.SetThreePoint)
                            setLoss += 1;
                    }
                }
            }

            //Game gained or loss calculation
            List<TennisMatch> gameMatches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o =>
             (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(request.otherUser)).ToList();

            if (gameMatches != null && gameMatches.Count() > 0)
            {
                foreach (var item in gameMatches)
                {
                    var meAsMember = item.MatchMembers.Where(x => x.MemberId == request.otherUser).FirstOrDefault();
                    if (meAsMember != null)
                    {
                        var otherMember = item.MatchMembers.Where(x => x.TeamCode != meAsMember.TeamCode).FirstOrDefault();

                        var setOneLoss = (meAsMember.SetOnePoint + otherMember.SetOnePoint) - meAsMember.SetOnePoint;
                        var setOneGain = meAsMember.SetOnePoint;
                        var setTwoLoss = (meAsMember.SetTwoPoint + otherMember.SetTwoPoint) - meAsMember.SetTwoPoint;
                        var setTwoGain = meAsMember.SetTwoPoint;
                        var setThreeLoss = (meAsMember.SetThreePoint + otherMember.SetThreePoint) - meAsMember.SetThreePoint;
                        var setThreeGain = meAsMember.SetThreePoint;

                        gameWon += ((int)setOneGain + (int)setTwoGain + (int)setThreeGain);
                        gameLoss += ((int)setOneLoss + (int)setTwoLoss + (int)setThreeLoss);
                    }
                }
            }

            return new MatchBalanceContract
            {
                lossMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == request.otherUser && x.IsMatchWon == false && x.TennisMatch.Status == MatchStatusEnum.Completed && x.TennisMatch.Status == MatchStatusEnum.Rated).ToList().Count(),
                wonMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == request.otherUser && x.IsMatchWon == true && x.TennisMatch.Status == MatchStatusEnum.Completed && x.TennisMatch.Status == MatchStatusEnum.Rated).ToList().Count(),
                setLoss = setWon,
                setWon = setLoss,
                gameWon = gameWon,
                gameLoss = gameLoss,
                pointGainedLoss = (double)pointGainedLoss,
            };
        }
        else
        {

            //Point gained or loss calculation
            List<TennisMatch> matches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o => o.MatchDateTime <= End_check_Date && o.MatchDateTime >= Start_check_Date
             && (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(userId)).ToList();

            if (matches != null && matches.Count() > 0)
            {
                foreach (var item in matches)
                {
                    var matchMember = item.MatchMembers.Where(x => x.MemberId == userId).FirstOrDefault();
                    if (matchMember.IsMatchWon == true)
                        pointGainedLoss += matchMember.GamePoint;
                    else
                        pointGainedLoss -= matchMember.GamePoint;
                }
            }

            //Set gained or loss calculation
            List<TennisMatch> setMatches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o =>
             (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(userId)).ToList();

            if (setMatches != null && setMatches.Count() > 0)
            {
                foreach (var item in setMatches)
                {
                    var meAsMember = item.MatchMembers.Where(x => x.MemberId == userId).FirstOrDefault();
                    if (meAsMember != null)
                    {
                        var otherMember = item.MatchMembers.Where(x => x.TeamCode != meAsMember.TeamCode).FirstOrDefault();

                        if (meAsMember.SetOnePoint > otherMember.SetOnePoint)
                            setWon += 1;
                        if (meAsMember.SetTwoPoint > otherMember.SetTwoPoint)
                            setWon += 1;
                        if (meAsMember.SetThreePoint > otherMember.SetThreePoint)
                            setWon += 1;
                        if (otherMember.SetOnePoint > meAsMember.SetOnePoint)
                            setLoss += 1;
                        if (otherMember.SetTwoPoint > meAsMember.SetTwoPoint)
                            setLoss += 1;
                        if (otherMember.SetThreePoint > meAsMember.SetThreePoint)
                            setLoss += 1;
                    }
                }
            }

            //Game gained or loss calculation
            List<TennisMatch> gameMatches = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(o =>
             (o.Status == MatchStatusEnum.Completed || o.Status == MatchStatusEnum.Rated) && o.MatchMembers.Select(x => x.MemberId).ToList().Contains(userId)).ToList();

            if (gameMatches != null && gameMatches.Count() > 0)
            {
                foreach (var item in gameMatches)
                {
                    var meAsMember = item.MatchMembers.Where(x => x.MemberId == userId).FirstOrDefault();
                    if (meAsMember != null)
                    {
                        var otherMember = item.MatchMembers.Where(x => x.TeamCode != meAsMember.TeamCode).FirstOrDefault();

                        var setOneLoss = (meAsMember.SetOnePoint + otherMember.SetOnePoint) - meAsMember.SetOnePoint;
                        var setOneGain = meAsMember.SetOnePoint;
                        var setTwoLoss = (meAsMember.SetTwoPoint + otherMember.SetTwoPoint) - meAsMember.SetTwoPoint;
                        var setTwoGain = meAsMember.SetTwoPoint;
                        var setThreeLoss = (meAsMember.SetThreePoint + otherMember.SetThreePoint) - meAsMember.SetThreePoint;
                        var setThreeGain = meAsMember.SetThreePoint;

                        gameWon += ((int)setOneGain + (int)setTwoGain + (int)setThreeGain);
                        gameLoss += ((int)setOneLoss + (int)setTwoLoss + (int)setThreeLoss);
                    }
                }
            }

            return new MatchBalanceContract
            {
                lossMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == userId && x.IsMatchWon == false && x.TennisMatch.Status == MatchStatusEnum.Completed && x.TennisMatch.Status == MatchStatusEnum.Rated).ToList().Count(),
                wonMatches = _context.MatchMembers.Include(x => x.TennisMatch).Where(x => x.MemberId == userId && x.IsMatchWon == true && x.TennisMatch.Status == MatchStatusEnum.Completed && x.TennisMatch.Status == MatchStatusEnum.Rated).ToList().Count(),
                setLoss = setWon,
                setWon = setLoss,
                gameWon = gameWon,
                gameLoss = gameLoss,
                pointGainedLoss = (double)pointGainedLoss,
            };
        }
    }
}