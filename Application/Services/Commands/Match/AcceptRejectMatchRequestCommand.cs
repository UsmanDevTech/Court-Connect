using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Domain.Enum;
using Domain.Entities;

namespace Application.Services.Commands;

public sealed record AcceptRejectMatchRequestCommand(int matchRequestId, StatusEnum status, string? paymentIntent, double? amountPaid) : IRequest<Result>;
internal sealed class AcceptRejectMatchRequestCommandHandler : IRequestHandler<AcceptRejectMatchRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public AcceptRejectMatchRequestCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser, IDateTime dateTime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _identityService = identityService;
    }
    public async Task<Result> Handle(AcceptRejectMatchRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var name = _identityService.GetUserName(userId);

        var matchRequestId = _context.MatchJoinRequests.Find(request.matchRequestId);
        if (matchRequestId == null)
            throw new NotFoundException("Invalid request id");

        //Check if already member added
        var memberExist = _context.MatchJoinRequests.Where(x => x.TennisMatchId == matchRequestId.TennisMatchId
        && x.RequestStatus == StatusEnum.Accepted).FirstOrDefault();
        if (memberExist != null)
            throw new CustomInvalidOperationException("Member already selected");
        
        var notifyDescription = "";
        var notifyTitle = "";
        var tennisMatch = _context.TennisMatches.Where(x => x.Id == matchRequestId.TennisMatchId).FirstOrDefault();

        
        //Add to match member if request accepted
        if (request.status == StatusEnum.Accepted)
        {
            SubscriptionHistory pck = null;

            if (request.paymentIntent == "")
            {
                if (tennisMatch.MatchCategory == GameTypeEnum.Unranked)
                    pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Unranked);
                else
                    pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Ranked);

                if (pck == null)
                    throw new CustomInvalidOperationException("Purchase subscription to create match!");
            }


            //Get Owner team code
            var ownerAsMember = _context.MatchMembers.Where(x => x.TennisMatchId == matchRequestId.TennisMatchId && x.CreatedBy == tennisMatch.CreatedBy
            && x.MemberId == tennisMatch.CreatedBy).FirstOrDefault();

            tennisMatch.AddMatchMember(tennisMatch.CreatedBy, userId, _dateTime.NowUTC, JoinTypeEnum.Request, ownerAsMember.TeamCode);

            //Cancel remaining pending status requests
            var memberList = _context.MatchJoinRequests.Where(x => x.TennisMatchId == matchRequestId.TennisMatchId && x.RequestStatus == StatusEnum.Pending && x.Id != request.matchRequestId).ToList();
            foreach (var member in memberList)
            {
                member.UpdateStatus(StatusEnum.Cancelled);
                _context.MatchJoinRequests.Update(member);
            }

            //Notification status update
            var notifications = _context.Notifications.Where(x => x.RedirectId == matchRequestId.TennisMatchId && x.RedirectType == NotificationRedirectEnum.ParticipantRequest  && x.NotifyTo == userId).ToList();
            foreach (var notification in notifications)
            {
                notification.UpdateReadStatus(true);
                _context.Notifications.Update(notification);
            }

            //Search for chat head
            var matchChatHead = _context.ChatHeads.Where(x => x.TennisMatchId == tennisMatch.Id).FirstOrDefault();

            if (matchChatHead == null)
                throw new NotFoundException("Chat head not found");

            //Add chat member
            matchChatHead.AddChatMember(userId, _dateTime.NowUTC);


            //Update remaining match count
            if (request.paymentIntent == "")
            {
                //Update the remaining match
                if (tennisMatch.MatchCategory == GameTypeEnum.Unranked)
                {

                    if (pck.RemainingFreeUnrankedGames == 0)
                        pck.RemainingFreeUnrankedGames = 0;
                    else
                        pck.RemainingFreeUnrankedGames -= 1;

                    _context.SubscriptionHistories.Update(pck);
                }
                else if (tennisMatch.MatchCategory == GameTypeEnum.Ranked)
                {
                    if (pck.RemainingFreeRankedGames == 0)
                        pck.RemainingFreeRankedGames = 0;
                    else
                        pck.RemainingFreeRankedGames -= 1;

                    _context.SubscriptionHistories.Update(pck);
                }
            }
            else
            {
                tennisMatch.AddMatchPurchase(userId, (double)request.amountPaid, 0.0, 0.0, request.paymentIntent, _dateTime.NowUTC);
            }

            //Match request accept notification
            notifyTitle = "Match Join Request Accepted";
            notifyDescription = name + " has accepted to your request as participant  for match" + tennisMatch.Title;

            _context.Notifications.AddAsync(Notification.Create(null, notifyTitle,
                notifyDescription, tennisMatch.CreatedBy, tennisMatch.Id, NotificationTypeEnum.Clickable,
                NotificationRedirectEnum.MatchDetail, tennisMatch.MatchCategory, _dateTime.NowUTC, userId));
        }
        else
        {
            //Match request reject notification
            notifyTitle = "Match Join Request Rejected";
            notifyDescription = name + " has rejected to your request as participant  for match" + tennisMatch.Title;

            _context.Notifications.AddAsync(Notification.Create(null, notifyTitle,
                notifyDescription, tennisMatch.CreatedBy, tennisMatch.Id, NotificationTypeEnum.Clickable,
                NotificationRedirectEnum.MatchDetail, tennisMatch.MatchCategory, _dateTime.NowUTC, userId));

            //Notification status update
            var singleNotification = _context.Notifications.Where(x => x.MatchJoinRequestId == matchRequestId.Id && x.NotifyTo == userId 
            && x.RedirectType == NotificationRedirectEnum.ParticipantRequest).FirstOrDefault();
            singleNotification.UpdateReadStatus(true);
            _context.Notifications.Update(singleNotification);
        }

        //Update match join request status
        matchRequestId.UpdateStatus(request.status);
        _context.MatchJoinRequests.Update(matchRequestId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}