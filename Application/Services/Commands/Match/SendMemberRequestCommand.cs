using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record SendMemberRequestCommand(string participantId, int matchId) : IRequest<Result>;
internal sealed class SendMemberRequestCommandHandler : IRequestHandler<SendMemberRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public SendMemberRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
       IIdentityService identityService, IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _identityService = identityService;
    }
    public async Task<Result> Handle(SendMemberRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var match = _context.TennisMatches.Include(x => x.MatchJoinRequests).Where(x => x.Id == request.matchId && x.Status == Domain.Enum.MatchStatusEnum.Initial).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");

        var memberSelected = match.MatchJoinRequests.Where(x => x.RequestStatus == Domain.Enum.StatusEnum.Accepted).FirstOrDefault();
        if (memberSelected != null)
            throw new CustomInvalidOperationException("Member already selected");

        var memberExist = _context.MatchJoinRequests.Where(x => x.TennisMatchId == request.matchId
            && x.MemberId == request.participantId).FirstOrDefault();

        if (memberExist != null)
        {
            if (memberExist.RequestStatus == Domain.Enum.StatusEnum.Accepted ||
                memberExist.RequestStatus == Domain.Enum.StatusEnum.Pending)
            {
                throw new CustomInvalidOperationException("Duplicate request");
            }

            else if (memberExist.RequestStatus == Domain.Enum.StatusEnum.Left ||
                memberExist.RequestStatus == Domain.Enum.StatusEnum.Rejected)
            {
                memberExist.RequestStatus = Domain.Enum.StatusEnum.Pending;

                var notification = _context.Notifications.Where(x => x.MatchJoinRequestId == memberExist.Id &&
                x.NotifyTo == request.participantId && x.RedirectType == Domain.Enum.NotificationRedirectEnum.ParticipantRequest).FirstOrDefault();

                notification.UpdateReadStatus(false);
                notification.UpdateCreatedAt(_dateTime.NowUTC);

                _context.Notifications.Update(notification);
                _context.MatchJoinRequests.Update(memberExist);
            }

        }
        else
        {
            //Remove previous notification start
            var notifications = _context.Notifications.Where(x => x.RedirectId == match.Id && x.IsSeen == false
              && x.RedirectType == Domain.Enum.NotificationRedirectEnum.ParticipantRequest && x.NotifyTo == request.participantId).ToList();

            foreach (var item in notifications)
            {
                item.UpdateReadStatus(true);
                _context.Notifications.Update(item);
            }
            //Remove previous notification end

            var Description = _identityService.GetUserName(match.CreatedBy) + " has invited you to be the partner for a match named " + match.Title + " do you want to join him?";
            match.SendParticipantRequest(userId, request.participantId, Description, (int)match.MatchCategory, _dateTime.NowUTC);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

