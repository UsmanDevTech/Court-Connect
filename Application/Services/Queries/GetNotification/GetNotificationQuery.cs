using Application.Common.Interfaces;
using Application.Common.Extensions;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;
public sealed record GetNotificationQuery(PaginationRequestBase request): IRequest<PaginationResponseBase<List<NotificationContract>>>;
internal sealed class GetNotificationQueryHandler : IRequestHandler<GetNotificationQuery, PaginationResponseBase<List<NotificationContract>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentLoginUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public GetNotificationQueryHandler(IApplicationDbContext context, ICurrentUserService currentLoginUser, IDateTime dateTime, IIdentityService identityService)
    {
        _context = context;
        _currentLoginUser = currentLoginUser;
        _identityService = identityService;
        _dateTime = dateTime;
    }
    public async Task<PaginationResponseBase<List<NotificationContract>>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var user = _currentLoginUser.UserId;
        var timezoneId = _identityService.GetTimezone(user);

        var validFilter = new PaginationRequestBase(request?.request?.pageNumber, request?.request?.pageSize);
        IQueryable<Notification> query = _context.Notifications.Where(u => (u.NotifyTo == _currentLoginUser.UserId && u.IsSeen == false) 
        || (u.NotifyTo == _currentLoginUser.UserId && u.IsSeen == true && (u.RedirectType != NotificationRedirectEnum.ParticipantRequest && u.RedirectType != NotificationRedirectEnum.ScoreVerification))).AsQueryable();
        
        var pagedData = await query
            .OrderByDescending(u => u.Created)
            .Select(u => new NotificationContract
            {
                id = u.Id,
                body = u.Description,
                title = u.Title,
                isSeen = u.IsSeen,
                createdAt = u.Created.UtcToLocalTime(timezoneId).ToString(_dateTime.longDateFormat),
                notifyBy =  _identityService.GetUserDetail(u.CreatedBy),
                notifyTo = _identityService.GetUserDetail(u.NotifyTo),
                status = u.RedirectType == NotificationRedirectEnum.ScoreVerification ?
                _context.MatchMembers.Where(x => x.MemberId == user && x.TennisMatchId == u.RedirectId).Select(x => x.IsScoreApproved).FirstOrDefault() : StatusEnum.None,
                type = u.Type,
                redirectType = u.RedirectType,
                redirectId = u.RedirectId,
                MatchJoinRequestId = u.MatchJoinRequestId,
                matchCategory = u.MatchCategory,
            })
            .Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
            .Take(validFilter.pageSize.Value)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        
        return PaginationHelper.CreatePagedReponse(pagedData, validFilter, totalRecords);
    }
}
