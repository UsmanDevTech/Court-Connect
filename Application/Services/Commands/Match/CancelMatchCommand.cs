using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record CancelMatchCommand(int matchid) : IRequest<Result>;
internal sealed class CancelMatchCommandHandler : IRequestHandler<CancelMatchCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTimeService;
    private readonly IIdentityService _identityService;
    public IFormatProvider provider { get; private set; }
    public CancelMatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTimeService, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTimeService = dateTimeService;
        _identityService = identityService;
    }
    public async Task<Result> Handle(CancelMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var name = _identityService.GetUserName(userId);

        //Tennis match
        var existingMatch = _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchid).FirstOrDefault();
        if (existingMatch == null)
            throw new NotFoundException("Match not found");

        if (existingMatch.CreatedBy != userId)
            throw new CustomInvalidOperationException("Only owner can cancel the match!");

        if (existingMatch.Status == MatchStatusEnum.Cancelled ||
          existingMatch.Status == MatchStatusEnum.Expired || existingMatch.Status == MatchStatusEnum.Completed)
            throw new CustomInvalidOperationException("Only active match can be cancelled!");

        DateTime date1 = DateTime.UtcNow;
        DateTime date2 = existingMatch.MatchDateTime - new TimeSpan(0, 15, 0);
        int result = DateTime.Compare(date1, date2);

        if (result > 0)
            throw new CustomInvalidOperationException("Match can not be cancelled as start time has passed");


        //Update pending Need to update the remaining count
        if ((existingMatch.MatchDateTime - DateTime.Now) > TimeSpan.FromHours(24))
        {
            //Refund match to owner
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

            ////Refund match to participants
            foreach (var item in existingMatch.MatchMembers.Where(x => x.MemberId != x.CreatedBy).ToList())
            {
                if (existingMatch.MatchCategory == GameTypeEnum.Unranked)
                {
                    var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                    pck.UpdateRemainingCount(0);

                    _context.SubscriptionHistories.Update(pck);
                }
                else
                {
                    var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                    pck.UpdateRemainingCount(1);

                    _context.SubscriptionHistories.Update(pck);
                }
            }
        }
        else
        {
            var title = "Warning ticket is issued by admin";
            var description = "A warning ticket is issued for the match " + existingMatch.Title + " as you cancel the match, when match start time was less then 24 hours";
            await _context.ProfileWarnings.AddAsync(ProfileWarning.Create(null, userId, title, description, _dateTimeService.NowUTC));

            //Return match count to participants
            if(existingMatch.MatchMembers != null && existingMatch.MatchMembers.Count() > 0)
            {
                foreach (var item in existingMatch.MatchMembers.Where(x=>x.MemberId != existingMatch.CreatedBy))
                {

                    if (existingMatch.MatchCategory == GameTypeEnum.Unranked)
                    {
                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                        pck.UpdateRemainingCount(0);

                        _context.SubscriptionHistories.Update(pck);
                    }
                    else
                    {
                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                        pck.UpdateRemainingCount(1);

                        _context.SubscriptionHistories.Update(pck);
                    }

                    title = "Match Cancelled";
                    description = name + " has cancelled the match" + existingMatch.Title;

                    if(item.MemberId != userId) { 
                    _context.Notifications.AddAsync(Notification.Create(null, title,
                        description, item.MemberId, existingMatch.Id, NotificationTypeEnum.Clickable,
                        NotificationRedirectEnum.MatchDetail, existingMatch.MatchCategory, _dateTimeService.NowUTC, userId));
                    }
                }
            }
        }


        //Send Notification Pending

        existingMatch.updateStatus(MatchStatusEnum.Cancelled);


        //Search for chat head
        var matchChatHead = _context.ChatHeads.Include(x => x.ChatConversations).Where(x => x.TennisMatchId == request.matchid).FirstOrDefault();

        if (matchChatHead == null)
            throw new NotFoundException("Chat head not found");

        if (matchChatHead.Deleted)
            matchChatHead.Deleted = true;


        var ChatMember = _context.ChatMembers.Where(c => c.IsChatHeadDeleted == false && c.ChatHeadId == matchChatHead.Id).ToList();

        if (ChatMember != null && ChatMember.Count() > 0)
        {
            foreach (var item in ChatMember)
            {
                item.isChatHeadDelete(true);

                var msgId = matchChatHead.ChatConversations.OrderBy(d => d.Created).LastOrDefault();
                if (msgId != null)
                    item.headDelete(msgId.Id);

                _context.ChatMembers.Update(item);
            }
        }

        _context.ChatHeads.Update(matchChatHead);
        _context.TennisMatches.Update(existingMatch);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
