using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Enum;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Services.Queries;

public sealed record GetMatchDetailByUserIdQuery(DataTablePaginationFilter filter, string userId) : IRequest<DatatableResponse<List<GenericPostQueryListModel<TennisMatchDtlsByUserContract>>>>;
internal sealed class GetMatchDetailByUserIdQueryHandler : IRequestHandler<GetMatchDetailByUserIdQuery, DatatableResponse<List<GenericPostQueryListModel<TennisMatchDtlsByUserContract>>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IIdentityService _identityService;
    public GetMatchDetailByUserIdQueryHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTime = dateTime;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<DatatableResponse<List<GenericPostQueryListModel<TennisMatchDtlsByUserContract>>>> Handle(GetMatchDetailByUserIdQuery request, CancellationToken cancellationToken)
    {

        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        IQueryable<GenericPreQueryListModel<TennisMatchDtlsByUserContract>> query =
            _context.TennisMatches.Include(x => x.MatchMembers)
            .Where(x => x.CreatedBy == request.userId).OrderByDescending(x => x.Created)
            .Select(e => new GenericPreQueryListModel<TennisMatchDtlsByUserContract>
            {
                createdAt = e.Created,
                lastUpdatedAt = e.Modified,
                createdBy = e.CreatedBy,
                updatedBy = e.ModifiedBy,
                data = new TennisMatchDtlsByUserContract
                {
                    id = e.Id,
                    startDateTime = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_dateTime.timeFormat),
                    status = e.Status,
                    address = e.Address,
                    latitute = e.Location.Y,
                    longitute = e.Location.X,
                    matchCategory = e.MatchCategory,
                    matchType = e.Type,
                    thumbnail = e.MatchImage,
                    title = e.Title,
                    creator = _identityService.GetUserDetail(e.CreatedBy),
                    description = e.Description,
                    created = e.Created.UtcToLocalTime(timezoneId).ToString(_dateTime.longDayDateTimeFormat),
                    deleted = e.Deleted,
                    startDateTimeDate = DateTime.UtcNow,
                    startDate = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_dateTime.dateFormat),
                    isJoined = false,
                    isMyMatch = false,
                    blockReason = e.BlockReason,
                }
            }).AsQueryable();

        //Implement filteration
        if (validFilter.status != null && validFilter.status != 0)
        {
            //Initial
            if (validFilter.status.Value == 1)
                query = query.Where(u => u.data.status == MatchStatusEnum.Initial).AsQueryable();
            //Wating
            else if (validFilter.status.Value == 2)
                query = query.Where(u => u.data.status == MatchStatusEnum.Waiting).AsQueryable();
            //ReadyToStart
            else if (validFilter.status.Value == 3)
                query = query.Where(u => u.data.status == MatchStatusEnum.ReadyToStart).AsQueryable();
            //Started
            else if (validFilter.status.Value == 4)
                query = query.Where(u => u.data.status == MatchStatusEnum.Started).AsQueryable();
            //Completed
            else if (validFilter.status.Value == 5)
                query = query.Where(u => u.data.status == MatchStatusEnum.Completed).AsQueryable();
            //Rated
            else if (validFilter.status.Value == 6)
                query = query.Where(u => u.data.status == MatchStatusEnum.Rated).AsQueryable();
            //Cancelled
            else if (validFilter.status.Value == 7)
                query = query.Where(u => u.data.status == MatchStatusEnum.Cancelled).AsQueryable();
            //Expired
            else if (validFilter.status.Value == 8)
                query = query.Where(u => u.data.status == MatchStatusEnum.Expired).AsQueryable();
            //InsufficientParticipant
            else if (validFilter.status.Value == 9)
                query = query.Where(u => u.data.status == MatchStatusEnum.InsufficientParticipant).AsQueryable();
            //Active
            else if (validFilter.status.Value == 10)
                query = query.Where(u => u.data.deleted == false).AsQueryable();
            //Blocked
            else if (validFilter.status.Value == 11)
                query = query.Where(u => u.data.deleted == true).AsQueryable();
        }

        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>
                u.data.title.Contains(searchString)
            );
        }
        //Sorting
        if (!string.IsNullOrEmpty(validFilter.sortColumn) && !string.IsNullOrEmpty(validFilter.sortColumnDirection))
        {
            query = query.OrderBy(validFilter.sortColumn + " " + validFilter.sortColumnDirection);
        }

        List<GenericPostQueryListModel<TennisMatchDtlsByUserContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<TennisMatchDtlsByUserContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt != null ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = _identityService.GetUserDetail(u.createdBy),
            updatedBy = _identityService.GetUserName(u.updatedBy),
            data = new TennisMatchDtlsByUserContract
            {
                id = u.data.id,
                startDateTime = u.data.startDateTime,
                status = u.data.status,
                address = u.data.address,
                latitute = u.data.latitute,
                longitute = u.data.longitute,
                matchCategory = u.data.matchCategory,
                matchType = u.data.matchType,
                thumbnail = u.data.thumbnail,
                title = u.data.title,
                creator = u.data.creator,
                description = u.data.description,
                created = u.data.created,
                deleted = u.data.deleted,
                isMyMatch = u.data.isMyMatch,
                isJoined = u.data.isJoined,
                startDate = u.data.startDate,
                startDateTimeDate = u.data.startDateTimeDate,
                blockReason = u.data.blockReason,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }
}