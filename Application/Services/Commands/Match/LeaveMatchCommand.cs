using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record LeaveMatchCommand(int matchId) : IRequest<Result>;
internal sealed class LeaveMatchCommandHandler : IRequestHandler<LeaveMatchCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTimeService;
    private readonly IIdentityService _identityService;
    public IFormatProvider provider { get; private set; }
    public LeaveMatchCommandHandler(IApplicationDbContext context,
        IDateTime dateTimeService, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTimeService = dateTimeService;
        _identityService = identityService;
    }
    public async Task<Result> Handle(LeaveMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var name = _identityService.GetUserName(userId);
        //Tennis match
        var existingMatch = _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchJoinRequests)
            .Where(x => x.Id == request.matchId).FirstOrDefault();
        if (existingMatch == null)
            throw new NotFoundException("Match not found");

        if (existingMatch.Status == MatchStatusEnum.Cancelled ||
            existingMatch.Status == MatchStatusEnum.Expired || existingMatch.Status == MatchStatusEnum.Completed)
            throw new CustomInvalidOperationException("Only active match can be left!");

        DateTime date1 = DateTime.UtcNow;
        DateTime date2 = existingMatch.MatchDateTime - new TimeSpan(0, 15, 0);
        int result = DateTime.Compare(date1, date2);

        if (result > 0)
            throw new CustomInvalidOperationException("Match can not be left as start time has passed");

        //Refund match to the participant
        if ((existingMatch.MatchDateTime - DateTime.Now) > TimeSpan.FromHours(24))
        {
            if (existingMatch.MatchCategory == GameTypeEnum.Unranked)
            {
                var pck = await _identityService.GetRefundPackageDetailAsync(userId);
                pck.UpdateRemainingCount(0);

                _context.SubscriptionHistories.Update(pck);
            }
            else
            {
                var pck = await _identityService.GetRefundPackageDetailAsync(userId);
                pck.UpdateRemainingCount(1);

                _context.SubscriptionHistories.Update(pck);
            }
        }
        else
        {
            var title = "Warning ticket is issued by admin";
            var description = "A warning ticket is issued for the match " + existingMatch.Title + " as you left the match, when match start time was less then 24 hours";
            await _context.ProfileWarnings.AddAsync(ProfileWarning.Create(null, userId, title, description, _dateTimeService.NowUTC));
        }

        //Match leave notification
        var notifyTitle = "Match left";
        var notifyDescription = name + " has left your match " + existingMatch.Title;

        _context.Notifications.AddAsync(Notification.Create(null, notifyTitle,
            notifyDescription, existingMatch.CreatedBy, existingMatch.Id, NotificationTypeEnum.Clickable,
            NotificationRedirectEnum.MatchDetail, existingMatch.MatchCategory, _dateTimeService.NowUTC, userId));


        var requestParticipant = _context.MatchJoinRequests.Where(x => x.MemberId == userId && x.TennisMatchId == request.matchId).FirstOrDefault();
        if (requestParticipant != null)
        {
            var notifications = _context.Notifications.Where(x => x.RedirectId == existingMatch.Id && x.IsSeen == false
                && x.RedirectType == NotificationRedirectEnum.ParticipantRequest && x.NotifyTo == userId).ToList();

            foreach (var item in notifications)
            {
                item.UpdateReadStatus(true);
                _context.Notifications.Update(item);
            }

            requestParticipant.UpdateStatus(StatusEnum.Left);
            _context.MatchJoinRequests.Update(requestParticipant);
        }

        var participant = _context.MatchMembers.Where(x => x.MemberId == userId && x.TennisMatchId == request.matchId).FirstOrDefault();
        if (participant != null)
        {
            existingMatch.RemoveMember(participant);
            existingMatch.IsMembersLimitFull = false;

            _context.TennisMatches.Update(existingMatch);
        }

        //Search for chat head
        var matchChatHead = _context.ChatHeads.Include(x => x.ChatMembers).Where(x => x.TennisMatchId == request.matchId).FirstOrDefault();

        if (matchChatHead == null)
            throw new NotFoundException("Chat head not found");

        var member = matchChatHead.ChatMembers.Where(x => x.ParticipantId == userId).FirstOrDefault();

        if (member != null)
            matchChatHead.RemoveChatMember(member);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}