using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.Generics;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Queries;

public sealed record GetChatHeadPaginationQuery(PaginationRequestBaseGeneric<RequestBaseContract> query) : IRequest<PaginationResponseBaseWithActionRequest<List<ChatHeadContract>>>;
internal sealed class GetChatHeadPaginationQueryHandler : IRequestHandler<GetChatHeadPaginationQuery, PaginationResponseBaseWithActionRequest<List<ChatHeadContract>>>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTimeService;
    public GetChatHeadPaginationQueryHandler(IIdentityService identityService,
         IApplicationDbContext context, ICurrentUserService currentUserService,
         IDateTime dateTimeService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _context = context;
        _dateTimeService = dateTimeService;
    }

    public async Task<PaginationResponseBaseWithActionRequest<List<ChatHeadContract>>> Handle(GetChatHeadPaginationQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        //Creating pagination filters according to request
        var validFilter = new PaginationRequestBase(request?.query?.pageNumber, request?.query?.pageSize);
        //count all records before filtering
        var totalRecords = 0;


        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.ChatHead).Count() < 1)
            throw new NotFoundException("Chat head type is required");

        var item = request.query.data.filters.Where(u => u.actionType == ActionFilterEnum.ChatHead).First();
        var chatType = Convert.ToInt32(item.val);

        var query = new List<ChatHead>();
        if (chatType == 0)
        {
            //Show other user matches with filter irrespective joined or not
            query = _context.ChatHeads.Include(x => x.TennisMatch)
                .Include(x => x.ChatConversations).Include(x => x.ChatMembers).AsEnumerable().Where(h => h.Type == ChatTypeEnum.Individual && h.Deleted == false &&
                  h.ChatMembers.Where(m => m.IsChatHeadDeleted == false && m.ParticipantId == userId).Any()).OrderByDescending(x => x.LastMessageTime).ToList();

            totalRecords = query.Count();
        }
        else
        {
            query = _context.ChatHeads.Include(x => x.TennisMatch)
                .Include(x => x.ChatConversations).Include(x => x.ChatMembers).AsEnumerable().Where(h => h.Type == ChatTypeEnum.Group && h.Deleted == false &&
            h.ChatMembers.Where(m => m.IsChatHeadDeleted == false && m.ParticipantId == userId).Any()).OrderByDescending(x => x.LastMessageTime).ToList();

            totalRecords = query.Count();
        }

        List<ChatHeadContract> head = query
                  .Select(e => new ChatHeadContract
                  {
                      chatHeadId = e.Id,
                      type = e.Type,
                      lastMsg = e.ChatConversations.OrderByDescending(x => x.Created).Select(y => new LastMessageContract
                      {
                          sender = _identityService.GetUserDetail(y.SenderId),
                          datetime = y.Created.UtcToLocalTime(timezoneId),
                          message = y.Message,
                          latitute = y.Latitute == null ? 0 : y.Latitute,
                          longitute = y.Longitue == null ? 0 : y.Longitue,
                          Type = y.MessageType,
                          messageId = y.Id,
                          isSeen = e.ChatMembers.Where(x => x.ParticipantId == userId).Select(x => x.LastMsgSeenId).FirstOrDefault() == y.Id ? true : false
                      }).FirstOrDefault(),
                      matchId = e.TennisMatchId,
                      matchTitle = e.TennisMatchId != null ? e.TennisMatch.Title : "---",
                      matchPic = e.TennisMatchId != null ? e.TennisMatch.MatchImage : "---",
                      users = e.ChatMembers.Select(x => _identityService.GetUserDetail(x.ParticipantId)).ToList(),
                  }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                    .Take(validFilter.pageSize.Value).ToList();

        //Filter by search
        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Count() > 0)
        {
            var searchValue = request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Select(u => u.val).First() ?? "";

            //Show own matches with filter
            //totalRecords = chatType == 0 ? query.Where(x => x.ChatMembers.Select(x => x.ParticipantId).ToList().Contains(searchValue.ToLower().Trim())).Count()
            //      : query.Where(h => h.TennisMatch.Title.ToLower().Contains(searchValue.ToLower().Trim())).Count();

            head = chatType == 1 ? head.Where(p => p.matchTitle.ToLower().Contains(searchValue.ToLower().Trim())).ToList()
            : head.Where(p => p.users.Select(x=>x.name.ToLower().Trim()).ToList().Contains(searchValue.ToLower().Trim())).ToList();

            totalRecords = head.Count();
        }

        var totalPages = ((double)totalRecords / (double)validFilter.pageSize.Value);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var respose = new PaginationResponseBaseWithActionRequest<List<ChatHeadContract>>(head, request.query.data, validFilter.pageNumber ?? 0, validFilter.pageSize ?? 0, roundedTotalPages, totalRecords);
        return respose;
    }
}