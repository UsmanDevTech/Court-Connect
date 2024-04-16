using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;

namespace Application.Services.Queries;

public sealed record DashboardStatsQuery() : IRequest<DashboardContract>;
internal sealed class DashboardStatsQueryHandler : IRequestHandler<DashboardStatsQuery, DashboardContract>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    public DashboardStatsQueryHandler(IIdentityService identityService,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<DashboardContract> Handle(DashboardStatsQuery request, CancellationToken cancellationToken)
    {
        return new DashboardContract
        {
            users = await _identityService.GetUsersCounterAsync(),
            couchingHubs = _context.CouchingHubs.ToList().Count(),
            leagues = _context.Leagues.ToList().Count(),
            matches = _context.TennisMatches.ToList().Count(),
        };
    }
}