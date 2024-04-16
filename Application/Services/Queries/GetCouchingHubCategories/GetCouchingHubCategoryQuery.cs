using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetCouchingHubCategoryQuery : IRequest<List<GenericIconInfoContract>>;
internal sealed class GetCouchingHubCategoryQueryHandler : IRequestHandler<GetCouchingHubCategoryQuery, List<GenericIconInfoContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetCouchingHubCategoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<List<GenericIconInfoContract>> Handle(GetCouchingHubCategoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return await _context.CouchingHubCategories.Where(x => x.Deleted == false).OrderByDescending(x => x.Created).Select(u => new GenericIconInfoContract
        {
            id = u.Id,
            name = u.Name,
            iconUrl = u.Icon,
        }).ToListAsync(cancellationToken);
    }
}