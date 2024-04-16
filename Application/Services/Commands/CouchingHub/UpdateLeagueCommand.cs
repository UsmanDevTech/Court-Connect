using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Commands;

public sealed class UpdateLeagueCommand : IRequest<Result>
{
    public int leagueId { get; set; }
    public string? leagueName { get; set; }
    public string? leagueIcon { get; set; }
    public int minRange { get; set; }
    public int maxRange { get; set; }
}
internal sealed class UpdateLeagueCommandHandler : IRequestHandler<UpdateLeagueCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityservice;
    public UpdateLeagueCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityservice)
    {
        _context = context;
        _currentUser = currentUser;
        _identityservice = identityservice;
    }

    public async Task<Result> Handle(UpdateLeagueCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var league = _context.Leagues.Where(x => x.Id == request.leagueId).FirstOrDefault();
        if (league == null)
            throw new NotFoundException("League not found");

        var iconvalue = "";
        var name = "";

        if (!string.IsNullOrEmpty(request.leagueIcon))
        {
            iconvalue = request.leagueIcon;
        }
        else
        {
            iconvalue = league.Icon;
        }

        if (!string.IsNullOrEmpty(request.leagueName))
        {
            name = request.leagueName;
        }
        else
        {
            name = league.Name;
        }

        league.UpdateLeague(iconvalue, name,request.minRange,request.maxRange);

        _context.Leagues.Update(league);
        await _context.SaveChangesAsync(cancellationToken);

        //Return Value
        return Result.Success();


    }
}
