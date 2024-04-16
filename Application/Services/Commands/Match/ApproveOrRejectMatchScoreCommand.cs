using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record ApproveOrRejectMatchScoreCommand(int matchId, StatusEnum status) : IRequest<Result>;
internal sealed class ApproveOrRejectMatchScoreCommandHandler : IRequestHandler<ApproveOrRejectMatchScoreCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public ApproveOrRejectMatchScoreCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ApproveOrRejectMatchScoreCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var tennisMatch = _context.TennisMatches.Include(x => x.MatchMembers)
            .Include(x => x.MatchJoinRequests)
            .Include(x => x.TemporaryMatchScores).Where(x => x.Id == request.matchId).FirstOrDefault();

        if (tennisMatch == null)
            throw new NotFoundException("Match not found!");

        var matchMember = tennisMatch.MatchMembers.Where(x => x.MemberId == userId).FirstOrDefault();
        if (matchMember == null)
            throw new NotFoundException("Only member can accept or reject score verification request!");

        //Verify score if request accepted
        if (request.status == StatusEnum.Accepted)
        {
            var notifications = _context.Notifications.Where(x => x.RedirectId == tennisMatch.Id && x.IsSeen == false
                 && x.RedirectType == NotificationRedirectEnum.ScoreVerification && x.NotifyTo == matchMember.MemberId).ToList();

            foreach (var item in notifications)
            {
                item.UpdateReadStatus(true);
                _context.Notifications.Update(item);
            }

            matchMember.updateIsScored(StatusEnum.Accepted);
        }
        else if (request.status == StatusEnum.Rejected) {
            var notifications = _context.Notifications.Where(x => x.RedirectId == tennisMatch.Id && x.IsSeen == false
               && x.RedirectType == NotificationRedirectEnum.ScoreVerification && x.NotifyTo == matchMember.MemberId).ToList();

            foreach (var item in notifications)
            {
                item.UpdateReadStatus(true);
                _context.Notifications.Update(item);
            }

            matchMember.updateIsScored(StatusEnum.Rejected);
        }

        _context.MatchMembers.Update(matchMember);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}