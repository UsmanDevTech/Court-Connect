using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Generics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Common.Extensions;
using Domain.Helpers;

namespace Application.Chat.Queries;

public sealed record GetChatByChatHeadIdPaginationQuery(PaginationRequestBase request, int chatHeadId) : IRequest<PaginationResponseBase<List<ConversationChat>>>;
internal sealed class GetChatByChatHeadIdPaginationQueryHandler : IRequestHandler<GetChatByChatHeadIdPaginationQuery, PaginationResponseBase<List<ConversationChat>>>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTimeService;
    private readonly IApplicationDbContext _context;
    public GetChatByChatHeadIdPaginationQueryHandler(IIdentityService identityService,
        ICurrentUserService currentUserService, IDateTime dateTimeService,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task<PaginationResponseBase<List<ConversationChat>>> Handle(GetChatByChatHeadIdPaginationQuery request, CancellationToken cancellationToken)
    {
        var user = _currentUserService.UserId;
        var timezoneId = _identityService.GetTimezone(user);

        var validFilter = new PaginationRequestBase(request?.request?.pageNumber, request?.request?.pageSize);

        var cht = await _context.ChatHeads.Where(x => x.Id == request.chatHeadId).FirstOrDefaultAsync();
        if (cht == null)
            throw new NotFoundException("Chat head not found!");

    
        var chatMsg = _context.ChatMembers.Where(p => p.ChatHeadId == request.chatHeadId && p.ParticipantId == user).FirstOrDefault();
        if (chatMsg != null)
        {
            if (chatMsg.ChatHeadDeleteLastMsgId != null)
            {
                //Show other user matches with filter irrespective joined or not
                IQueryable<ChatConversation> query = _context.ChatConversations.Where(b => b.ChatHeadId == request.chatHeadId && b.Id > chatMsg.ChatHeadDeleteLastMsgId).OrderByDescending(o => o.Created).AsQueryable();

                var chat = await query
                          .Select(e => new ConversationChat
                          {
                              chatHeadId = cht.Id,
                              chatId = e.Id,
                              sender = _identityService.GetUserDetail(e.SenderId),
                              sendTime = e.Created.UtcToLocalTime(timezoneId),
                              latitute = e.Latitute,
                              longitute = e.Longitue,
                              message = e.Message,
                              messageType = e.MessageType,
                          }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                            .Take(validFilter.pageSize.Value)
                            .ToListAsync(cancellationToken);
                
                chat.Reverse();
                var totalRecords = await query.CountAsync(cancellationToken);
                return PaginationHelper.CreatePagedReponse(chat, validFilter, totalRecords);
            }

            else
            {
                //Show other user matches with filter irrespective joined or not
                IQueryable<ChatConversation> query = _context.ChatConversations.Where(b => b.ChatHeadId == request.chatHeadId).OrderByDescending(o => o.Created).AsQueryable();

               var chat = await query
                          .Select(e => new ConversationChat
                          {
                              chatHeadId = cht.Id,
                              chatId = e.Id,
                              sender = _identityService.GetUserDetail(e.SenderId),
                              sendTime = e.Created.UtcToLocalTime(timezoneId),
                              latitute = e.Latitute,
                              longitute = e.Longitue,
                              message = e.Message,
                              messageType = e.MessageType,
                          }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                            .Take(validFilter.pageSize.Value)
                            .ToListAsync(cancellationToken);

                chat.Reverse();
                var totalRecords = await query.CountAsync(cancellationToken);
                return PaginationHelper.CreatePagedReponse(chat, validFilter, totalRecords);
            }
        }
        else
        {
            throw new CustomInvalidOperationException("Member not found!");
        }
    }
}