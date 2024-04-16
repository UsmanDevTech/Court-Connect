using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;


namespace Application.Services.Queries;

public sealed record GetLeaguePaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<GenericPostQueryListModel<AdminLeagueContract>>>>;
internal sealed class GetLeaguePaginationQueryHandler : IRequestHandler<GetLeaguePaginationQuery, DatatableResponse<List<GenericPostQueryListModel<AdminLeagueContract>>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IIdentityService _identityService;
    public GetLeaguePaginationQueryHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTime = dateTime;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<DatatableResponse<List<GenericPostQueryListModel<AdminLeagueContract>>>> Handle(GetLeaguePaginationQuery request, CancellationToken cancellationToken)
    {

        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        IQueryable<GenericPreQueryListModel<AdminLeagueContract>> query =
            _context.Leagues.OrderByDescending(x => x.Created)
            .Select(e => new GenericPreQueryListModel<AdminLeagueContract>
            {
                createdAt = e.Created,
                lastUpdatedAt = e.Modified,
                createdBy = e.CreatedBy,
                updatedBy = e.ModifiedBy,
                data = new AdminLeagueContract
                {
                    id = e.Id,
                    icon = e.Icon,
                    name = e.Name,
                    maxRange = e.MaxRange,
                    minRange = e.MinRange,
                    deleted = e.Deleted,
                    created = "",
                }
            }).AsQueryable();

        //Implement filteration
        if (validFilter.status != null && validFilter.status != 0)
        {
            //Active
            if (validFilter.status.Value == 1)
                query = query.Where(u => u.data.deleted == false).AsQueryable();
            //Blocked
            else if (validFilter.status.Value == 2)
                query = query.Where(u => u.data.deleted == true).AsQueryable();
        }

        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>u.data.name.Contains(searchString));
        }
        //Sorting
        if (!string.IsNullOrEmpty(validFilter.sortColumn) && !string.IsNullOrEmpty(validFilter.sortColumnDirection))
        {
            query = query.OrderBy(validFilter.sortColumn + " " + validFilter.sortColumnDirection);
        }

        List<GenericPostQueryListModel<AdminLeagueContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<AdminLeagueContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt != null ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = new LeagueRankingUserContract(),
            updatedBy = "",
            data = new AdminLeagueContract
            {
                id = u.data.id,
                icon = u.data.icon,
                name = u.data.name,
                maxRange = u.data.maxRange,
                minRange = u.data.minRange,
                deleted = u.data.deleted,
                created = u.data.created,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }
}