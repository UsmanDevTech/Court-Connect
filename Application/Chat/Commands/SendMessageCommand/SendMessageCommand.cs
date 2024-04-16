using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Commands;

public sealed class SendMessageCommand : IRequest<Result>
{
    public int chatHeadId { get; set; }
    public string? message { get; set; }
    public MediaTypeEnum messageType { get; set; }
    public double? latitute { get; set; }
    public double? longitute { get; set; }
}
internal sealed class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    private readonly IChatHub _chatHubService;
    public SendMessageCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser, IDateTime dateTime, IIdentityService identityService,
        IChatHub chatHubService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _identityService = identityService;
        _chatHubService = chatHubService;
    }
    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var name = _identityService.GetUserName(userId);

        var timezoneId = _identityService.GetTimezone(userId);

        var chatHead = _context.ChatHeads.Include(x => x.TennisMatch).Include(x => x.ChatMembers).Where(c => c.Id == request.chatHeadId).FirstOrDefault();

        if (chatHead == null)
            throw new NotFoundException("Chat head not found");

        if (chatHead.Deleted)
            chatHead.Deleted = false;


        var ChatMember = _context.ChatMembers.Where(c => c.IsChatHeadDeleted == true && c.ChatHeadId == request.chatHeadId).ToList();
        if (ChatMember != null && ChatMember.Count() > 0)
        {
            foreach (var item in ChatMember)
            {
                item.isChatHeadDelete(false);
                _context.ChatMembers.Update(item);
            }
        }

        ChatConversation chat = chatHead.SendMessage(userId, request.latitute, request.longitute, request.message,
                  request.messageType, _dateTime.NowUTC);

        _context.ChatConversations.AddAsync(chat);

        chatHead.LastMessageTime = chat.Created;
        _context.ChatHeads.Update(chatHead);

        await _context.SaveChangesAsync(cancellationToken);

        //Send notification for message receive
        if (chatHead.ChatMembers != null && chatHead.ChatMembers.Count() > 0)
        {
            foreach (var item in chatHead.ChatMembers.Where(x => x.ParticipantId != userId).ToList())
            {
                var notifyTitle = "New Message Received";
                var notifyDescription = "";

                if (chatHead.Type == ChatTypeEnum.Individual)
                    notifyDescription = "New message received from" + name;
                else
                    notifyDescription = "New message received in the group" + chatHead.TennisMatch.Title;

                var matchCategory = chatHead.Type == ChatTypeEnum.Individual ? GameTypeEnum.None : chatHead.TennisMatch.MatchCategory;
                _context.Notifications.AddAsync(Notification.Create(null, notifyTitle,
                    notifyDescription, item.ParticipantId, null, NotificationTypeEnum.Clickable,
                    NotificationRedirectEnum.ChatMessage, matchCategory, _dateTime.NowUTC, userId));
            }
        }

        //Send message via hub

        var chatObject = new ConversationChat
        {
            chatHeadId = request.chatHeadId,
            chatId = chat.Id,
            sender = _identityService.GetUserDetail(userId),
            sendTime = chat.Created.UtcToLocalTime(timezoneId),
            latitute = request.latitute,
            longitute = request.longitute,
            message = request.message,
            messageType = request.messageType,
        };

        await _chatHubService.SendMessage(chatObject);
        return Result.Success();

    }
}