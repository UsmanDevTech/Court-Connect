using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Generics;
using MediatR;

namespace Application.Services.Queries;

public sealed record GetTennisDetailPaginationQuery(PaginationRequestBaseGeneric<RequestBaseContract> query) : IRequest<PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>>;
internal sealed class GetTennisDetailPaginationQueryHandler : IRequestHandler<GetTennisDetailPaginationQuery, PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>>
{
    private readonly IIdentityService _identityService;
    public GetTennisDetailPaginationQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>> Handle(GetTennisDetailPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetTennisMatches(request, cancellationToken);
    }
}
