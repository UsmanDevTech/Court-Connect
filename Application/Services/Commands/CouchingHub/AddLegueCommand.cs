using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;

namespace Application.Services.Commands;

public sealed class AddLeagueCommand : IRequest<Result>
{
    public string leagueName { get; set; }
    public string leagueIcon { get; set; }
    public int minRange { get; set; }
    public int maxRange { get; set; }
    public List<AddSubLeague> subleagues { get; set; }
}
public class AddSubLeague
{
    public string subleagueName { get; set; }
    public string subleagueIcon { get; set; }
    public int subminRange { get; set; }
    public int submaxRange { get; set; }
}

internal sealed class AddLeagueCommandHandler : IRequestHandler<AddLeagueCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityservice;
    private readonly IDateTime _datetime;
    public string[] dateFormats = new[] { "yyyy/MM/dd", "MM/dd/yyyy", "MM/dd/yyyyHH:mm:ss" };
    public IFormatProvider provider { get; set; }
    public AddLeagueCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityservice, IDateTime datetime)
    {
        _context = context;
        _currentUser = currentUser;
        _identityservice = identityservice;
        _datetime = datetime;
    }

    public async Task<Result> Handle(AddLeagueCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var league = Domain.Entities.League.Create(request.leagueIcon, request.leagueName, request.minRange, request.maxRange, _currentUser.UserId, _datetime.NowUTC);
        _context.Leagues.Add(league);

        if (request.subleagues.Count() > 0)
        {
            foreach(var sub in request.subleagues)
            {
                league.AddSubLeague(sub.subleagueIcon, sub.subleagueName, sub.subminRange, sub.submaxRange, _datetime.NowUTC, false);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();


    }
}
