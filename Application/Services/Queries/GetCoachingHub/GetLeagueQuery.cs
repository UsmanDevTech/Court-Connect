﻿using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Queries;

public sealed record GetLeagueQuery(int leaqueId) : IRequest<LeagueContract>;
internal sealed class GetLeagueQueryHandler : IRequestHandler<GetLeagueQuery, LeagueContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetLeagueQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<LeagueContract> Handle(GetLeagueQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return await _context.Leagues.Where(x => x.Id == request.leaqueId).Select(u => new LeagueContract
        {
            id = u.Id,
            name = u.Name,
            icon = u.Icon,
            minRange = u.MinRange,
            maxRange = u.MaxRange,
            created = u.Created.ToString("dd--mm-yyyy"),
        }).FirstOrDefaultAsync(cancellationToken);
    }
}
