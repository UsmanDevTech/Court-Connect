using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Commands;

public sealed record AddGroupMemberCommand(string participantId, int matchId) : IRequest<ChatHeadContract>;
internal sealed class AddGroupMemberCommandHandler : IRequestHandler<AddGroupMemberCommand, ChatHeadContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public AddGroupMemberCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser, IDateTime dateTime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _identityService = identityService;
    }
    public async Task<ChatHeadContract> Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var timezoneId = _identityService.GetTimezone(userId);

        //Search for chat head
        var matchChatHead = _context.ChatHeads.Include(x => x.ChatMembers)
            .Include(x => x.ChatConversations).Where(x => x.TennisMatchId == request.matchId).FirstOrDefault();

        if (matchChatHead == null)
            throw new NotFoundException("Chat head not found");

        if (matchChatHead.ChatMembers.Where(x => x.ParticipantId == userId).Any())
        {
            return await _context.ChatHeads.Include(x => x.ChatMembers)
            .Include(x => x.ChatConversations).Where(x => x.TennisMatchId == request.matchId).Select(x => new ChatHeadContract
            {
                chatHeadId = x.Id,
                type = x.Type,
                matchId = x.TennisMatchId,
                matchTitle = x.TennisMatchId != null ? x.TennisMatch.Title : "---",
                matchPic = x.TennisMatchId != null ? x.TennisMatch.MatchImage : "---",
                lastMsg = _context.ChatConversations.Where(e => e.ChatHeadId == x.Id).OrderByDescending(x => x.Created).Select(y => new LastMessageContract
                {
                    sender = _identityService.GetUserDetail(y.SenderId),
                    datetime = y.Created.UtcToLocalTime(timezoneId),
                    message = y.Message,
                    latitute = y.Latitute == null ? 0 : y.Latitute,
                    longitute = y.Longitue == null ? 0 : y.Longitue,
                    Type = y.MessageType,
                    messageId = y.Id,
                }).FirstOrDefault(),
                users = x.ChatMembers.Select(x => _identityService.GetUserDetail(x.ParticipantId)).ToList(),
            }).FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            //Add chat member
            matchChatHead.AddChatMember(userId, _dateTime.NowUTC);
            await _context.SaveChangesAsync(cancellationToken);


            return await _context.ChatHeads.Include(x => x.ChatMembers)
            .Include(x => x.ChatConversations).Where(x => x.TennisMatchId == request.matchId).Select(x => new ChatHeadContract
            {
                chatHeadId = x.Id,
                type = x.Type,
                matchId = x.TennisMatchId,
                matchTitle = x.TennisMatchId != null ? x.TennisMatch.Title : "---",
                matchPic = x.TennisMatchId != null ? x.TennisMatch.MatchImage : "---",
                lastMsg = _context.ChatConversations.Where(e => e.ChatHeadId == x.Id).OrderByDescending(x => x.Created).Select(y => new LastMessageContract
                {
                    sender = _identityService.GetUserDetail(y.SenderId),
                    datetime = y.Created.UtcToLocalTime(timezoneId),
                    message = y.Message,
                    latitute = y.Latitute == null ? 0 : y.Latitute,
                    longitute = y.Longitue == null ? 0 : y.Longitue,
                    Type = y.MessageType,
                    messageId = y.Id,
                }).FirstOrDefault(),
                users = x.ChatMembers.Select(x => _identityService.GetUserDetail(x.ParticipantId)).ToList(),
            }).FirstOrDefaultAsync(cancellationToken);
        }

        
    }
}