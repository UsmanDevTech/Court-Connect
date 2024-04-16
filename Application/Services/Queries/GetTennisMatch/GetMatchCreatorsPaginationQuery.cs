using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using MediatR;

namespace Application.Services.Queries;

public sealed record GetMatchCreatorsPaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>>;

public sealed class GetMatchCreatorsPaginationQueryHandler : IRequestHandler<GetMatchCreatorsPaginationQuery, DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>>
{
    private readonly IIdentityService _identityService;
   
    public GetMatchCreatorsPaginationQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>> Handle(GetMatchCreatorsPaginationQuery request, CancellationToken cancellationToken)
    {

        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);
        return await _identityService.GetMatchCreatorDetailAsync(validFilter, cancellationToken);
    }
}