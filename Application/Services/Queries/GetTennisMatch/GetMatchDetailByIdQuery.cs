
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetMatchDetailByIdQuery(int matchId) : IRequest<TennisMatchDtlsContract>;
internal sealed class GetMatchDetailByIdQueryHandler : IRequestHandler<GetMatchDetailByIdQuery, TennisMatchDtlsContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetMatchDetailByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<TennisMatchDtlsContract> Handle(GetMatchDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);
        var myCode = _identityService.GetTeamCode(userId, request.matchId);

        var ownerTeam = _context.MatchMembers.Where(x => x.MemberId == x.CreatedBy && x.TennisMatchId == request.matchId).FirstOrDefault();

        return await _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.Id == request.matchId).Select(e => new TennisMatchDtlsContract
        {
            id = e.Id,
            
            teamA = e.MatchMembers.Where(x => x.TeamCode == ownerTeam.TeamCode).Select(x=> new TeamContract
            {
                teamCode = x.TeamCode,
                member = _identityService.GetUserDetail(x.MemberId),
            }).ToList(),
            teamB = e.MatchMembers.Where(x => x.TeamCode != ownerTeam.TeamCode).Select(x => new TeamContract
            {
                teamCode = x.TeamCode,
                member = _identityService.GetUserDetail(x.MemberId),
            }).ToList(),

            isMyMatch = e.CreatedBy == userId ? true : false,
            isJoined = e.MatchMembers.Where(x => x.MemberId == userId).Any(),
            startDate = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.dayFormat),
            startDateTime = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateTimeFormat),
            startDateTimeDate = e.MatchDateTime.UtcToLocalTime(timezoneId),
            status = e.Status,
            address = e.Address,
            latitute = e.Location.Y,
            longitute = e.Location.X,
            level = (LevelEnum)_identityService.GetUserLevel(e.CreatedBy),
            matchCategory = e.MatchCategory,
            matchType = e.Type,
            thumbnail = e.MatchImage,
            title = e.Title,
            creator = _identityService.GetUserDetail(e.CreatedBy),
            description = e.Description,
            myPartner = myCode != "" ?e.MatchMembers.Where(x => x.TeamCode == myCode && x.MemberId != userId).Select(x => _identityService.GetUserDetail(x.MemberId)).FirstOrDefault()
            : null,
            participants = e.MatchMembers.Select(x => _identityService.GetUserDetail(x.MemberId)).ToList(),
            requestedUsers = _identityService.GetMatchRequestUserDetail(e.Id, e.CreatedBy),
        }).FirstOrDefaultAsync(cancellationToken) ?? new TennisMatchDtlsContract();
    }
}