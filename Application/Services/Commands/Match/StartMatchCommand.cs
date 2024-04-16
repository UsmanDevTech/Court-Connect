using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using Domain.Enum;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Threading;

namespace Application.Services.Commands;

public sealed record StartMatchCommand(int matchid) : IRequest<Result>;
internal sealed class StartMatchCommandHandler : IRequestHandler<StartMatchCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IChatHub _chatHubService;
    public IFormatProvider provider { get; private set; }
    public StartMatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IIdentityService identityService, IChatHub chatHubService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _chatHubService = chatHubService;
    }
    public async Task<Result> Handle(StartMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        //Tennis match
        var existingMatch = _context.TennisMatches.Include(x=>x.MatchLocations).Include(x => x.MatchMembers).Where(x => x.Id == request.matchid).FirstOrDefault();
        if (existingMatch == null)
            throw new NotFoundException("Match not found");

        if (existingMatch.CreatedBy != userId)
            throw new CustomInvalidOperationException("Only owner can start the match!");

        if (existingMatch.Status == MatchStatusEnum.Cancelled ||
            existingMatch.Status == MatchStatusEnum.Expired || existingMatch.Status == MatchStatusEnum.Completed)
            throw new CustomInvalidOperationException("Only active match can be started!");

        DateTime date1 = DateTime.UtcNow;
        DateTime date2 = existingMatch.MatchDateTime - new TimeSpan(0, 15, 0);
        int result = DateTime.Compare(date1, date2);

        if (result < 0)
            throw new CustomInvalidOperationException("Match can not start before match start time!");


        //Send location request via hub

        //var matchObject = new LocationVerificationContract
        //{
        //    matchId = existingMatch.Id,
        //    participants = existingMatch.MatchMembers.Select(x => x.MemberId).ToList(),
        //};

        //await _chatHubService.SendVerification(matchObject);

        //Wait for response
        //Thread.Sleep(60000);

        //var locationCheck = true;
        //foreach (var item in existingMatch.MatchMembers)
        //{
        //    Point userLocation = _context.MatchLocations.Where(x => x.TennisMatchId == existingMatch.Id && x.UserId == item.MemberId).Select(x => x.Location).FirstOrDefault();
        //    if(userLocation != null)
        //    {
        //        if ((existingMatch.Location.ProjectTo(2855).Distance(userLocation.ProjectTo(2855)) / 1000) > 2)
        //            locationCheck = false;
        //    }
        //    else
        //    {
        //        locationCheck = false;
        //    }
        //}
        //if (locationCheck)
        //{
            existingMatch.Status = MatchStatusEnum.Started;
            _context.TennisMatches.Update(existingMatch);
        //}
        //else
        //{
        //    throw new CustomInvalidOperationException("All participants have not arrived at the location!");
        //}

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
