using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Commands;

public sealed class UpdateMatchCommand : IRequest<Result>
{
    public int id { get; set; }
    public int type { get; set; }
    public int matchCategory { get; set; }
    public string startTime { get; set; } = null!;
    public double latitute { get; set; }
    public double longitute { get; set; }
    public string address { get; set; } = null!;
    public string thumbnail { get; set; } = null!;
    public string title { get; set; } = null!;
    public string? description { get; set; }
}
internal sealed class UpdateMatchCommandHandler : IRequestHandler<UpdateMatchCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    public IFormatProvider provider { get; private set; }
    public UpdateMatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var timezoneId = _identityService.GetTimezone(userId);

        //Existing tennis match
        var existingMatch = _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.id).FirstOrDefault();
        if (existingMatch == null)
            throw new NotFoundException("Match not found");

        else if (existingMatch.Status != MatchStatusEnum.Initial)
            throw new CustomInvalidOperationException("Match can not be edited now");

        else if (existingMatch.MatchMembers.Count > 1)
            throw new CustomInvalidOperationException("Match can not be edited now as members as joined the match");

        DateTime date1 = DateTime.UtcNow;
        DateTime date2 = existingMatch.MatchDateTime - new TimeSpan(0, 15, 0);
        int result = DateTime.Compare(date1, date2);
       
        if (result > 0)
            throw new CustomInvalidOperationException("Match can not be edited as start time has passed");

        DateTime validTill;
        DateTime dateTime10; // 1/1/0001 12:00:00 AM  
        bool isSuccess = DateTime.TryParseExact(request.startTime, new string[] { "MM.dd.yyyy hh:mm tt", "MM-dd-yyyy hh:mm tt", "MM/dd/yyyy hh:mm tt" }, provider, DateTimeStyles.None, out dateTime10);
        if (isSuccess)
            validTill = DateTime.ParseExact(request.startTime, new string[] { "MM.dd.yyyy hh:mm tt", "MM-dd-yyyy hh:mm tt", "MM/dd/yyyy hh:mm tt" }, provider, DateTimeStyles.None);
        else
            throw new CustomInvalidOperationException("MM.dd.yyyy hh:mm tt, MM-dd-yyyy hh:mm tt, MM/dd/yyyy hh:mm tt these formats are supported only!");


        validTill = validTill.LocalToUtc(timezoneId);


        existingMatch.UpdateMatch(request.title, request.description,
             request.thumbnail, (MatchTypeEnum)request.type, (GameTypeEnum)request.matchCategory,
             request.latitute, request.longitute, request.address, validTill, _dateTime.NowUTC);

        _context.TennisMatches.Update(existingMatch);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}