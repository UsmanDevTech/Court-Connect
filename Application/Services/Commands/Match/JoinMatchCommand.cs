
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record JoinMatchCommand(int matchId, string? paymentIntent, double? amountPaid) : IRequest<Result>;
internal sealed class JoinMatchCommandHandler : IRequestHandler<JoinMatchCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IChatHub _chatHubService;
    private readonly IIdentityService _identityService;
    public JoinMatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime, IChatHub chatHubService, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _chatHubService = chatHubService;
        _identityService = identityService;
    }
    public async Task<Result> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var name = _identityService.GetUserName(userId);

        var match = _context.TennisMatches.Include(x=>x.MatchMembers).Where(x => x.Id == request.matchId && x.Status == Domain.Enum.MatchStatusEnum.Initial).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");

        if (match.IsMembersLimitFull == true)
            throw new CustomInvalidOperationException("Members limit is full!");

        if (match.Type == MatchTypeEnum.Single
            && match.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius).Count() == 1)
            throw new CustomInvalidOperationException("Participant is selected!");

        else if (match.Type != MatchTypeEnum.Single
            && match.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius).Count() == 2)
            throw new CustomInvalidOperationException("Participants are selected!");

        var memberAdded = _context.MatchMembers.Where(x => x.TennisMatchId == request.matchId && x.MemberId == userId).FirstOrDefault();
        if (memberAdded != null)
            throw new CustomInvalidOperationException("Duplicate request!");

        if (match.Type == MatchTypeEnum.Single)
        {
            match.IsMembersLimitFull = true;
        }
        else if (match.Type == MatchTypeEnum.Double || match.Type == MatchTypeEnum.Mixed)
        {
            if (match.MatchMembers.Count() == 3) match.IsMembersLimitFull = true;
        }

        _context.TennisMatches.Update(match);

        //Get team member code
        var memberCode = "";
        var teamMember = _context.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius &&
        x.TennisMatchId == request.matchId).FirstOrDefault();
        if (teamMember != null)
            memberCode = teamMember.TeamCode;

        SubscriptionHistory pck = null;

        if (request.paymentIntent == "")
        {
            if (match.MatchCategory == GameTypeEnum.Unranked)
                pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Unranked);
            else
                pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Ranked);

            if (pck == null)
                throw new CustomInvalidOperationException("Purchase subscription to create match!");
        }


        //Add member to the game
        match.AddMatchMember(match.CreatedBy, userId, _dateTime.NowUTC, JoinTypeEnum.Radius, memberCode);


        if (request.paymentIntent == "")
        {
            //Update the remaining match
            if (match.MatchCategory == GameTypeEnum.Unranked)
            {

                if (pck.RemainingFreeUnrankedGames == 0)
                    pck.RemainingFreeUnrankedGames = 0;
                else
                    pck.RemainingFreeUnrankedGames -= 1;

                _context.SubscriptionHistories.Update(pck);
            }
            else if (match.MatchCategory == GameTypeEnum.Ranked)
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
            match.AddMatchPurchase(userId, (double)request.amountPaid, 0.0, 0.0, request.paymentIntent, _dateTime.NowUTC);
        }

        //Search for chat head
        var matchChatHead = _context.ChatHeads.Where(x => x.TennisMatchId == request.matchId).FirstOrDefault();

        if (matchChatHead == null)
            throw new NotFoundException("Chat head not found");

        //Add chat member
        matchChatHead.AddChatMember(userId, _dateTime.NowUTC);

        //Match join notification
        var notifyTitle = "Member Joined The Match";
        var notifyDescription = name + " has joined as opponent to your match " + match.Title;

        _context.Notifications.AddAsync(Notification.Create(null, notifyTitle,
            notifyDescription, match.CreatedBy, match.Id, NotificationTypeEnum.Clickable,
            NotificationRedirectEnum.MatchDetail, match.MatchCategory, _dateTime.NowUTC, userId));

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
