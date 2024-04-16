using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;


public sealed record GetSubLeagueByLeagueIdQuery(int leagueId) : IRequest<List<SubleagueStatusContract>>;
internal sealed class GetSubLeagueByLeagueIdQueryHandler : IRequestHandler<GetSubLeagueByLeagueIdQuery, List<SubleagueStatusContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTimeService;
    public GetSubLeagueByLeagueIdQueryHandler(IApplicationDbContext context, IIdentityService identityService,
        IDateTime dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _identityService = identityService;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<List<SubleagueStatusContract>> Handle(GetSubLeagueByLeagueIdQuery request, CancellationToken cancellationToken)
    {
        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);

        return await _context.SubLeagues.Include(x => x.League).Where(x => x.LeagueId == request.leagueId && x.Deleted == false).Select(x => new SubleagueStatusContract
        {
            id = x.Id,
            icon = x.Icon,
            maxRange = x.MaxRange,
            minRange = x.MinRange,
            name = x.Name,
            points = new List<GenericIconInfoContract>(),
            status = x.Deleted,
            created = x.Created.UtcToLocalTime(timezoneId).ToString(_dateTimeService.dayFormat),
        }).ToListAsync(cancellationToken);
    }
}