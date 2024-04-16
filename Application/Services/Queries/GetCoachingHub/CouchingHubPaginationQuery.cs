using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Services.Queries;

public sealed record CouchingHubPaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<GenericPostQueryListModel<AdminCouchingHubContract>>>>;
internal sealed class CouchingHubPaginationQueryHandler : IRequestHandler<CouchingHubPaginationQuery, DatatableResponse<List<GenericPostQueryListModel<AdminCouchingHubContract>>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IIdentityService _identityService;
    public CouchingHubPaginationQueryHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTime = dateTime;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<DatatableResponse<List<GenericPostQueryListModel<AdminCouchingHubContract>>>> Handle(CouchingHubPaginationQuery request, CancellationToken cancellationToken)
    {

        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        IQueryable<GenericPreQueryListModel<AdminCouchingHubContract>> query =
            _context.CouchingHubs.Include(x => x.CouchingHubCategory).Include(x => x.CouchingHubDetails)
            .OrderByDescending(x => x.Created)
            .Select(e => new GenericPreQueryListModel<AdminCouchingHubContract>
            {
                createdAt = e.Created,
                lastUpdatedAt = e.Modified,
                createdBy = e.CreatedBy,
                updatedBy = e.ModifiedBy,
                data = new AdminCouchingHubContract
                {
                    id = e.Id,
                    type = e.Type,
                    title = e.CouchingHubDetails.Select(x => x.Title).FirstOrDefault(),
                    thumbnail = e.CouchingHubDetails.Select(x => x.Thumbnail).FirstOrDefault(),
                    url = e.CouchingHubDetails.Select(x => x.Url).FirstOrDefault(),
                    categoryId = e.CouchingHubCategoryId,
                    category = e.CouchingHubCategory.Name,
                    description = e.CouchingHubDetails.Select(x => x.Description).FirstOrDefault(),
                    mediaType = e.CouchingHubDetails.Select(x => x.Type).FirstOrDefault(),
                    price = e.Price == null ? 0 : e.Price,
                    deleted = e.Deleted,
                    date = e.Created.UtcToLocalTime(timezoneId).ToString(_dateTime.longDayDateTimeFormat),
                }
            }).AsQueryable();

        //Implement filteration
        if (validFilter.status != null && validFilter.status != 0)
        {
            //Free
            if (validFilter.status.Value == 1)
                query = query.Where(u => u.data.type == Domain.Enum.CouchingHubContentTypeEnum.Free).AsQueryable();
            //Paid
            else if (validFilter.status.Value == 2)
                query = query.Where(u => u.data.type == Domain.Enum.CouchingHubContentTypeEnum.Payable).AsQueryable();
            //Active
            if (validFilter.status.Value == 3)
                query = query.Where(u => u.data.deleted == false).AsQueryable();
            //Blocked
            else if (validFilter.status.Value == 4)
                query = query.Where(u => u.data.deleted == true).AsQueryable();
        }

        //Searching
        if (!string.IsNullOrEmpty(validFilter.searchValue))
        {
            string? searchString = validFilter?.searchValue.Trim();
            query = query.Where(u =>
                u.data.title.Contains(searchString) ||
                u.data.category.Contains(searchString)
            );
        }
        //Sorting
        if (!string.IsNullOrEmpty(validFilter.sortColumn) && !string.IsNullOrEmpty(validFilter.sortColumnDirection))
        {
            query = query.OrderBy(validFilter.sortColumn + " " + validFilter.sortColumnDirection);
        }

        List<GenericPostQueryListModel<AdminCouchingHubContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<AdminCouchingHubContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt != null ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = new LeagueRankingUserContract(),
            updatedBy = "",
            data = new AdminCouchingHubContract
            {
                id = u.data.id,
                type = u.data.type,
                title = u.data.title,
                thumbnail = u.data.thumbnail,
                url = u.data.url,
                categoryId = u.data.categoryId,
                category = u.data.category,
                description = u.data.description,
                mediaType = u.data.mediaType,
                price = u.data.price,
                deleted = u.data.deleted,
                date = u.data.date,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }
}