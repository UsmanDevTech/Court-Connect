using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetLeagueInformationQuery : IRequest<List<LeagueSubLeagueDetailContract>>;
internal sealed class GetLeagueInformationQueryHandler : IRequestHandler<GetLeagueInformationQuery, List<LeagueSubLeagueDetailContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetLeagueInformationQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<List<LeagueSubLeagueDetailContract>> Handle(GetLeagueInformationQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return await _context.Leagues.Include(x=>x.SubLeagues).Where(x => x.Deleted == false).OrderByDescending(x => x.Created).Select(u => new LeagueSubLeagueDetailContract
        {
            id = u.Id,
            name = u.Name,
            icon = u.Icon,
            minRange = u.MinRange,
            maxRange = u.MaxRange,
            created = u.Created.UtcToLocalTime(timezoneId).ToString(_datetime.longDateFormat),
            subLeagues = u.SubLeagues.Where(x => x.Deleted == false).Select(x => new SubLeagueContract
            {
                id = x.Id,
                name = x.Name,
                icon = x.Icon,
                maxRange = x.MaxRange,
                minRange = x.MinRange,
                created = u.Created.UtcToLocalTime(timezoneId).ToString(_datetime.longDateFormat),
                points = x.LeagueRewards.Select(x => new GenericIconInfoContract
                {
                    id = x.Id,
                    name = x.Detail,
                    iconUrl = x.Icon,
                }).ToList(),
            }).ToList(),
        }).ToListAsync(cancellationToken);
    }
}