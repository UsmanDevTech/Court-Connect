using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Commands;

public sealed record CreateChatHeadCommand(string secondUserId) : IRequest<ChatHeadContract>;
internal sealed class CreateChatHeadCommandHandler : IRequestHandler<CreateChatHeadCommand, ChatHeadContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    private readonly IChatHub _chatHubService;
    public CreateChatHeadCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser, IDateTime dateTime, IIdentityService identityService,
        IChatHub chatHubService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _identityService = identityService;
        _chatHubService = chatHubService;
    }
    public async Task<ChatHeadContract> Handle(CreateChatHeadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var timezoneId = _identityService.GetTimezone(userId);

        var SecondUser = _identityService.VerifyUser(request.secondUserId);
        if (SecondUser)
            throw new NotFoundException("Invalid user id!");

        List<ChatMember> firstUser = _context.ChatMembers.Include(x => x.ChatHead).Where(c => c.ChatHead.Type == ChatTypeEnum.Individual && c.ParticipantId == userId).ToList();
        List<ChatMember> secondUser = _context.ChatMembers.Include(x => x.ChatHead).Where(c => c.ChatHead.Type == ChatTypeEnum.Individual && c.ParticipantId == request.secondUserId).ToList();

        ChatMember chatHead = new ChatMember();
       
        if ((firstUser != null && firstUser.Count > 0) && (secondUser != null && secondUser.Count > 0))
        {
            var isHeadFound = false;

            foreach (var item in firstUser)
            {
                if (secondUser.Select(x => x.ChatHeadId).ToList().Contains(item.ChatHeadId) && isHeadFound == false)
                {
                    chatHead = item;
                    isHeadFound = true;
                }
            }

            if (isHeadFound == true)
            {
                return await _context.ChatHeads.Include(x => x.TennisMatch).Include(x => x.ChatMembers).Where(x => x.Id == chatHead.ChatHeadId).Select(x => new ChatHeadContract
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
                ChatHead createdChatHead = ChatHead.Create(ChatTypeEnum.Individual, null, userId, _dateTime.NowUTC);
                createdChatHead.AddChatMember(userId, _dateTime.NowUTC);
                createdChatHead.AddChatMember(request.secondUserId, _dateTime.NowUTC);
                ChatConversation conversation = createdChatHead.SendMessage(userId, 0, 0, "Hi", MediaTypeEnum.Text, _dateTime.NowUTC);

                _context.ChatConversations.Add(conversation);

                var chatObject = new ConversationChat
                {
                    chatHeadId = createdChatHead.Id,
                    chatId = conversation.Id,
                    sender = _identityService.GetUserDetail(userId),
                    sendTime = conversation.Created.UtcToLocalTime(timezoneId),
                    latitute = conversation.Latitute,
                    longitute = conversation.Longitue,
                    message = conversation.Message,
                    messageType = conversation.MessageType,
                };

                await _chatHubService.SendMessage(chatObject);

                _context.ChatHeads.Add(createdChatHead);
                await _context.SaveChangesAsync(cancellationToken);
               //await _chatHubService.SendChatHead(createdChatHead.Id, 0);

                return await _context.ChatHeads.Include(x => x.TennisMatch).Include(x => x.ChatMembers).Where(x => x.Id == createdChatHead.Id).Select(x => new ChatHeadContract
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
        else
        {
            ChatHead createdChatHead = ChatHead.Create(ChatTypeEnum.Individual, null, userId, _dateTime.NowUTC);
            createdChatHead.AddChatMember(userId, _dateTime.NowUTC);
            createdChatHead.AddChatMember(request.secondUserId, _dateTime.NowUTC);

            ChatConversation conversation = createdChatHead.SendMessage(userId, 0, 0, "Hi", MediaTypeEnum.Text, _dateTime.NowUTC);

            _context.ChatConversations.Add(conversation);

            var chatObject = new ConversationChat
            {
                chatHeadId = createdChatHead.Id,
                chatId = conversation.Id,
                sender = _identityService.GetUserDetail(userId),
                sendTime = conversation.Created.UtcToLocalTime(timezoneId),
                latitute = conversation.Latitute,
                longitute = conversation.Longitue,
                message = conversation.Message,
                messageType = conversation.MessageType,
            };

            await _chatHubService.SendMessage(chatObject);

            _context.ChatHeads.Add(createdChatHead);
            await _context.SaveChangesAsync(cancellationToken);

            //await _chatHubService.SendChatHead(createdChatHead.Id, 0);

            return await _context.ChatHeads.Include(x => x.TennisMatch).Include(x => x.ChatMembers).Where(x => x.Id == createdChatHead.Id).Select(x => new ChatHeadContract
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