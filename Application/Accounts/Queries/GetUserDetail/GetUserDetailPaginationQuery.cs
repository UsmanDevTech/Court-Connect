using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using MediatR;

namespace Application.Accounts.Queries;

public sealed record GetUserDetailPaginationQuery(DataTablePaginationFilter filter) : IRequest<DatatableResponse<List<UserProfileInfoDetailStatusContract>>>;
internal sealed class GetUserDetailPaginationQueryHandler : IRequestHandler<GetUserDetailPaginationQuery, DatatableResponse<List<UserProfileInfoDetailStatusContract>>>
{
    private readonly IIdentityService _identityService;

    public GetUserDetailPaginationQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<DatatableResponse<List<UserProfileInfoDetailStatusContract>>> Handle(GetUserDetailPaginationQuery request, CancellationToken cancellationToken)
    {
        var validFilter = new DataTablePaginationFilter(request?.filter?.pageNumber, request?.filter?.pageSize, request?.filter?.status, request?.filter?.sortColumn, request?.filter?.sortColumnDirection, request?.filter?.searchValue);

        return await _identityService.GetAllUsersDetailAsync(validFilter, cancellationToken);
    }
}