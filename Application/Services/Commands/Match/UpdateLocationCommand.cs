using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Services.Commands;

public sealed record UpdateLocationCommand(int matchId, string address, double longitute, double latitute) : IRequest<Result>;
internal sealed class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTimeService;

    public UpdateLocationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTimeService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var match = _context.TennisMatches.Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match Not Found");

        var existingUserLocation = _context.MatchLocations.Where(x => x.UserId == userId
        && x.TennisMatchId == request.matchId).FirstOrDefault();

        if(existingUserLocation != null)
        {
            existingUserLocation.UpdateLocation(request.address, request.latitute, request.longitute);
            _context.MatchLocations.Update(existingUserLocation);
        }
        else
        {
            match.UpdateLocation(match.CreatedBy, userId, request.latitute, request.longitute, request.address, _dateTimeService.NowUTC);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
