using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Application.Subscriptions.Queries;

public sealed record SubscriptionPackagesPaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<GenericPostQueryListModel<AllSubscriptionContract>>>>;
internal sealed class SubscriptionPackagesPaginationQueryHandler : IRequestHandler<SubscriptionPackagesPaginationQuery, DatatableResponse<List<GenericPostQueryListModel<AllSubscriptionContract>>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IIdentityService _identityService;
    public SubscriptionPackagesPaginationQueryHandler(IApplicationDbContext context, IDateTime dateTime, IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _dateTime = dateTime;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<DatatableResponse<List<GenericPostQueryListModel<AllSubscriptionContract>>>> Handle(SubscriptionPackagesPaginationQuery request, CancellationToken cancellationToken)
    {

        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        IQueryable<GenericPreQueryListModel<AllSubscriptionContract>> query =
            _context.Subscriptions.OrderByDescending(x => x.Created)
            .Select(e => new GenericPreQueryListModel<AllSubscriptionContract>
            {
                createdAt = e.Created,
                lastUpdatedAt = e.Modified,
                createdBy = e.CreatedBy,
                updatedBy = e.ModifiedBy,
                data = new AllSubscriptionContract
                {
                    id = e.Id,
                    title = e.Title,
                    price = e.Price,
                    priceAfterDiscount = e.PriceAfterDiscount,
                    discount = e.Discount,
                    isDiscountAvailable = e.IsDiscountAvailable,
                    durationType = e.DurationType,
                    subscriptionType = e.SubscriptionType,

                    isFreeCouchingContentAvailable = e.IsFreeCouchingContentAvailable,
                    isPaidCouchingContentAvailable = e.IsPaidCouchingContentAvailable,
                    costPerRankedGame = e.CostPerRankedGame,
                    costPerUnrankedGame = e.CostPerUnrankedGame,

                    isFreeRankedGameUnlimited = e.IsFreeRankedUnlimited,
                    isFreeUnrankedGameUnlimited = e.IsFreeUnrankedUnlimited,
                    freeRankedGames = e.FreeRankedGames,
                    freeUnrankedGames = e.FreeUnrankedGames,

                    isScoreAvailable = e.IsScoreAvailable,
                    isReviewAvailable = e.IsReviewsAvailable,
                    isMatchBalanceAvailable = e.IsMatchBalanceAvailable,
                    isRatingAvailable = e.IsRatingAvailable,
                    created = e.Created.UtcToLocalTime(timezoneId).ToString(_dateTime.longDayDateTimeFormat),
                    deleted = e.Deleted,
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
            query = query.Where(u =>
                u.data.title.Contains(searchString)
            );
        }
        //Sorting
        if (!string.IsNullOrEmpty(validFilter.sortColumn) && !string.IsNullOrEmpty(validFilter.sortColumnDirection))
        {
            query = query.OrderBy(validFilter.sortColumn + " " + validFilter.sortColumnDirection);
        }

        List<GenericPostQueryListModel<AllSubscriptionContract>> pagedData = await query.Select(u => new GenericPostQueryListModel<AllSubscriptionContract>
        {
            createdAt = u.createdAt.Value.ToString(_dateTime.shortDateFormat),
            lastUpdatedAt = u.lastUpdatedAt != null ? u.lastUpdatedAt.Value.ToString(_dateTime.shortDateFormat) : "N/A",
            createdBy = new LeagueRankingUserContract(),
            updatedBy = "",
            data = new AllSubscriptionContract
            {
                id = u.data.id,
                title = u.data.title,
                price = u.data.price,
                priceAfterDiscount = u.data.priceAfterDiscount,
                discount = u.data.discount,
                isDiscountAvailable = u.data.isDiscountAvailable,
                durationType = u.data.durationType,
                subscriptionType = u.data.subscriptionType,

                isFreeCouchingContentAvailable = u.data.isFreeCouchingContentAvailable,
                isPaidCouchingContentAvailable = u.data.isPaidCouchingContentAvailable,
                costPerRankedGame = u.data.costPerRankedGame,
                costPerUnrankedGame = u.data.costPerUnrankedGame,

                isFreeRankedGameUnlimited = u.data.isFreeRankedGameUnlimited,
                isFreeUnrankedGameUnlimited = u.data.isFreeUnrankedGameUnlimited,
                freeRankedGames = u.data.freeRankedGames,
                freeUnrankedGames = u.data.freeUnrankedGames,

                isScoreAvailable = u.data.isScoreAvailable,
                isReviewAvailable = u.data.isReviewAvailable,
                isMatchBalanceAvailable = u.data.isMatchBalanceAvailable,
                isRatingAvailable = u.data.isRatingAvailable,
                created = u.data.created,
                deleted = u.data.deleted,
            }
        }).Skip(validFilter.pageNumber ?? 0).Take(validFilter.pageSize ?? 0)
            .ToListAsync(cancellationToken);

        var totalRecords = await query.CountAsync(cancellationToken);
        var pagedReponse = PaginationHelper.CreateDatatableReponse(pagedData, validFilter, totalRecords);
        return pagedReponse;
    }
}