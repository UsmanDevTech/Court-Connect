using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetCreatedJoinedMatchesQuery() : IRequest<TennisJoinedCreatedMatchContract>;
internal sealed class GetCreatedJoinedMatchesQueryHandler : IRequestHandler<GetCreatedJoinedMatchesQuery, TennisJoinedCreatedMatchContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetCreatedJoinedMatchesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<TennisJoinedCreatedMatchContract> Handle(GetCreatedJoinedMatchesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);

        return new TennisJoinedCreatedMatchContract
        {
            created = _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews).AsEnumerable()
                  .Where(u => u.CreatedBy == _currentUser.UserId
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == _currentUser.UserId).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == _currentUser.UserId).Count() < 2))
                && u.Status != MatchStatusEnum.Cancelled && u.Deleted == false
                && u.Status != MatchStatusEnum.Rated
                && u.Status != MatchStatusEnum.Expired).OrderBy(x => x.MatchDateTime).Take(2).Select(x => new TennisMatchContract
                {
                    id = x.Id,
                    isJoined = x.MatchMembers.Where(x => x.MemberId == userId).Any(),
                    startDate = x.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.dayFormat),
                    startDateTime = x.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                    startDateTimeDate = x.MatchDateTime,
                    status = x.Status,
                    address = x.Address,
                    isMyMatch = x.CreatedBy == userId ? true : false,
                    latitute = x.Location.X,
                    longitute = x.Location.Y,
                    level = (LevelEnum)_identityService.GetUserLevel(x.CreatedBy),
                    matchCategory = x.MatchCategory,
                    matchType = x.Type,
                    thumbnail = x.MatchImage,
                    title = x.Title,
                }).ToList(),

            joined = _context.TennisMatches.Include(x => x.MatchMembers).Include(x => x.MatchReviews)
            .AsEnumerable().Where(u => u.CreatedBy != _currentUser.UserId && u.Deleted == false
               && u.MatchMembers.Where(x => x.MemberId == userId).Any()
                && ((u.Type == MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == _currentUser.UserId).Count() < 1) ||
                (u.Type != MatchTypeEnum.Single && u.MatchReviews.Where(x => x.CreatedBy == _currentUser.UserId).Count() < 2))
                && u.Status != MatchStatusEnum.Cancelled
                && u.Status != MatchStatusEnum.Rated
                && u.Status != MatchStatusEnum.Expired).OrderBy(x => x.MatchDateTime).Take(2).Select(x => new TennisMatchContract
                {
                    id = x.Id,
                    isJoined = x.MatchMembers.Where(x => x.MemberId == userId).Any(),
                    startDate = x.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.dayFormat),
                    startDateTime = x.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                    startDateTimeDate = x.MatchDateTime,
                    status = x.Status,
                    address = x.Address,
                    isMyMatch = x.CreatedBy == userId ? true : false,
                    latitute = x.Location.Y,
                    longitute = x.Location.X,
                    level = (LevelEnum)_identityService.GetUserLevel(x.CreatedBy),
                    matchCategory = x.MatchCategory,
                    matchType = x.Type,
                    thumbnail = x.MatchImage,
                    title = x.Title,
                }).ToList(),
        };
    }
}