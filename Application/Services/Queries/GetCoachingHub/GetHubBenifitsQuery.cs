using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetHubBenifitsQuery(int hubId) : IRequest<List<GenericIconInfoContract>>;
internal sealed class GetHubBenifitsQueryHandler : IRequestHandler<GetHubBenifitsQuery, List<GenericIconInfoContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetHubBenifitsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<List<GenericIconInfoContract>> Handle(GetHubBenifitsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return await _context.CouchingHubBenifits.Where(x => x.CouchingHubId == request.hubId).OrderByDescending(x => x.Created).Select(u => new GenericIconInfoContract
        {
            id = u.Id,
            name = u.Detail,
            iconUrl = u.Icon,
        }).ToListAsync(cancellationToken);
    }
}