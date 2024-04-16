using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Queries;

public sealed record GetWarningDetailPaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<GenericPostQueryListModel<UserWarningContract>>>>;
internal sealed class GetWarningDetailPaginationQueryHandler : IRequestHandler<GetWarningDetailPaginationQuery, DatatableResponse<List<GenericPostQueryListModel<UserWarningContract>>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IIdentityService _identityService;
    public GetWarningDetailPaginationQueryHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTime = dateTime;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<DatatableResponse<List<GenericPostQueryListModel<UserWarningContract>>>> Handle(GetWarningDetailPaginationQuery request, CancellationToken cancellationToken)
    {

        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        IQueryable<GenericPreQueryListModel<UserWarningContract>> query =
            _context.ProfileWarnings
            .OrderByDescending(x => x.Created)
            .Select(e => new GenericPreQueryListModel<UserWarningContract>
            {
                createdAt = e.Created,
                lastUpdatedAt = e.Modified,
                createdBy = e.CreatedBy,
                updatedBy = e.ModifiedBy,
                data = new UserWarningContract
                {
                    id = e.Id,
                    title = e.Title,
                    description = e.Description,
                    reportedBy = _identityService.GetUserDetail(e.CreatedBy),
                    reportedTo = _identityService.GetUserDetail(e.IssuedUser),
                }
            }).AsQueryable();

       

        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>
                u.data.title.Contains(searchString)
            //    u.data.reportedBy.name.Contains(searchString) ||
            //    u.data.reportedTo.name.Contains(searchString) ||
            //    u.data.reportedBy.email.Contains(searchString) ||
            //    u.data.reportedTo.email.Contains(searchString)
            );
        }

        List<GenericPostQueryListModel<UserWarningContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<UserWarningContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt != null ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = new LeagueRankingUserContract(),
            updatedBy = "",
            data = new UserWarningContract
            {
                id = u.data.id,
                title = u.data.title,
                description = u.data.description,
                reportedBy = u.data.reportedBy,
                reportedTo = u.data.reportedTo,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }
}
