using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.Generics;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetCoachingHubPaginationQuery(PaginationRequestBaseGeneric<RequestBaseContract> query) : IRequest<PaginationResponseBaseWithActionRequest<List<CouchingHubContract>>>;
internal sealed class GetCoachingHubPaginationQueryHandler : IRequestHandler<GetCoachingHubPaginationQuery, PaginationResponseBaseWithActionRequest<List<CouchingHubContract>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTimeService;
    public GetCoachingHubPaginationQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService, IDateTime dateTimeService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _dateTimeService = dateTimeService;
    }

    public async Task<PaginationResponseBaseWithActionRequest<List<CouchingHubContract>>> Handle(GetCoachingHubPaginationQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        //Creating pagination filters according to request
        var validFilter = new PaginationRequestBase(request?.query?.pageNumber, request?.query?.pageSize);

        List<CouchingHubContract> couchingHub = new List<CouchingHubContract>();

        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.ContentType).Count() < 1)
            throw new NotFoundException("Content type is required");

        var item = request.query.data.filters.Where(u => u.actionType == ActionFilterEnum.ContentType).First();
        var contentType = Convert.ToInt32(item.val);

        var totalRecords = contentType == 0 ? await _context
                .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Free && u.Deleted == false).CountAsync() :
                await _context
         .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Payable && u.Deleted == false).CountAsync();

        List<CouchingHub> query = contentType == 0 ? await _context
                  .CouchingHubs.Include(x => x.couchingHubBenifits).Include(x => x.CouchingHubDetails).Include(x => x.CouchingHubCategory).Include(x => x.PurchasedClassess)
                  .Where(u => u.Type == CouchingHubContentTypeEnum.Free && u.Deleted == false).ToListAsync() :
                   await _context
                  .CouchingHubs.Include(x => x.couchingHubBenifits).Include(x => x.CouchingHubDetails).Include(x => x.CouchingHubCategory).Include(x => x.PurchasedClassess)
                  .Where(u => u.Type == CouchingHubContentTypeEnum.Payable && u.Deleted == false).ToListAsync();


        couchingHub = query.OrderByDescending(x => x.Created)
                  .Select(e => new CouchingHubContract
                  {
                      id = e.Id,
                      type = e.Type,
                      title = e.Title,
                      price = e.Price,
                      isPurchased = e.PurchasedClassess.Where(x => x.CreatedBy == userId).FirstOrDefault() != null ? true : e.Type == CouchingHubContentTypeEnum.Free ? true : false,
                      category = new GenericIconInfoContract
                      {
                          id = e.CouchingHubCategory.Id,
                          iconUrl = e.CouchingHubCategory.Icon,
                          name = e.CouchingHubCategory.Name,
                      },
                      content = e.CouchingHubDetails.Select(y => new CouchingHubDetailContract
                      {
                          id = y.Id,
                          type = y.Type,
                          title = y.Title,
                          description = y.Description,
                          thumbnail = y.Thumbnail,
                          url = y.Url,
                          timeLength = y.TimeLength,
                      }).FirstOrDefault(),
                      benifits = e.couchingHubBenifits.Select(y => new GenericIconInfoContract
                      {
                          id = y.Id,
                          name = y.Detail,
                          iconUrl = y.Icon,
                      }).ToList(),
                      date = e.Created.UtcToLocalTime(timezoneId).ToString(_dateTimeService.longDateFormat),
                  }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                    .Take(validFilter.pageSize.Value).ToList();


        //Category filter if search action type found
        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Category).Count() > 0)
        {

            var fieldVal = request.query.data.filters.Where(u => u.actionType == ActionFilterEnum.Category).First();

            List<int> categories = new();

            foreach (var val in fieldVal.val.Split(','))
            {
                categories.Add(Convert.ToInt32(val));
            }

            if (categories.Count > 0)
            {
                List<CouchingHubContract> list2 = new List<CouchingHubContract>();
                foreach (var category in categories)
                {
                    var list1 = couchingHub.Where(u => u.category.id == category).ToList();

                    if (list1 != null && list1.Count > 0)
                        list2.AddRange(list1);
                }
                couchingHub = list2;

                totalRecords = contentType == 0 ? await _context
                  .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Free && u.Deleted == false &&
                   list2.Select(x => x.id).ToList().Contains(u.Id)).CountAsync() :
                  await _context
                  .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Payable && u.Deleted == false &&
                  list2.Select(x => x.id).ToList().Contains(u.Id)).CountAsync();
            }
        }

        //Filter by search
        if (request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Count() > 0)
        {
            var searchValue = request.query.data?.filters.Where(u => u.actionType == ActionFilterEnum.Search).Select(u => u.val).First() ?? "";

            totalRecords = contentType == 0 ? await _context
                  .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Free
                  && u.Deleted == false).CountAsync() :
                  await _context
           .CouchingHubs.Where(u => u.Type == CouchingHubContentTypeEnum.Payable && u.Deleted == false
           && u.Title.ToLower().Contains(searchValue.ToLower().Trim())).CountAsync();


            couchingHub = couchingHub.Where(p => p.title.ToLower().Contains(searchValue.ToLower().Trim())).ToList();
        }

        var totalPages = ((double)totalRecords / (double)validFilter.pageSize.Value);
        int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var respose = new PaginationResponseBaseWithActionRequest<List<CouchingHubContract>>(couchingHub, request.query.data, validFilter.pageNumber ?? 0, validFilter.pageSize ?? 0, roundedTotalPages, totalRecords);
        return respose;
    }
}
