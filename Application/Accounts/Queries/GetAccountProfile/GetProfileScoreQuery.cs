using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.Generics;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Accounts.Queries;

public sealed record GetProfileScoreQuery(PaginationRequestBase request, string? otherUser) : IRequest<PaginationResponseBase<List<ProfileScoreContract>>>;
internal sealed class GetProfileScoreQueryHandler : IRequestHandler<GetProfileScoreQuery, PaginationResponseBase<List<ProfileScoreContract>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetProfileScoreQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<PaginationResponseBase<List<ProfileScoreContract>>> Handle(GetProfileScoreQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);


        var validFilter = new PaginationRequestBase(request?.request?.pageNumber, request?.request?.pageSize);

        if (request.otherUser != "" && request.otherUser != null)
        {
            IQueryable<TennisMatch> query = _context.TennisMatches.Include(x => x.MatchMembers).AsEnumerable().Where(x => x.MatchMembers.Select(x => x.MemberId).Contains(request.otherUser) && x.Status == MatchStatusEnum.Completed).AsQueryable();

            var pagedData = await query.Select(e => new ProfileScoreContract
            {
                matchId = e.Id,
                teamA = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.None || x.MatchJoinType == JoinTypeEnum.Request).Select(x => new ProfileTeamScoreContract
                {
                    isMatchWon = x.IsMatchWon,
                    teamCode = x.TeamCode,
                    members = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.None || x.MatchJoinType == JoinTypeEnum.Request)
                   .Select(x => _identityService.GetUserDetail(x.MemberId)).ToList(),
                }).FirstOrDefault() ?? new(),
                teamB = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius).Select(x => new ProfileTeamScoreContract
                {
                    isMatchWon = x.IsMatchWon,
                    teamCode = x.TeamCode,
                    members = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius)
                    .Select(x => _identityService.GetUserDetail(x.MemberId)).ToList(),
                }).FirstOrDefault() ?? new(),

                startDateTime = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                creator = _identityService.GetUserDetail(e.CreatedBy),
            }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                  .Take(validFilter.pageSize.Value)
                  .ToListAsync(cancellationToken);

            var totalRecords = await query.CountAsync(cancellationToken);
            return PaginationHelper.CreatePagedReponse(pagedData, validFilter, totalRecords);
        }
        else
        {
            IQueryable<TennisMatch> query = _context.TennisMatches.Include(x => x.MatchMembers).Where(x => x.MatchMembers.Select(x => x.MemberId).Contains(userId) && x.Status == MatchStatusEnum.Completed).AsQueryable();

            var pagedData = await query.Select(e => new ProfileScoreContract
            {
                matchId = e.Id,
                teamA = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.None || x.MatchJoinType == JoinTypeEnum.Request).Select(x => new ProfileTeamScoreContract
                {
                    isMatchWon = x.IsMatchWon,
                    teamCode = x.TeamCode,
                    members = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.None || x.MatchJoinType == JoinTypeEnum.Request)
                    .Select(x => _identityService.GetUserDetail(x.MemberId)).ToList(),
                }).FirstOrDefault(),
                teamB = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius).Select(x => new ProfileTeamScoreContract
                {
                    isMatchWon = x.IsMatchWon,
                    teamCode = x.TeamCode,
                    members = e.MatchMembers.Where(x => x.MatchJoinType == JoinTypeEnum.Radius)
                    .Select(x => _identityService.GetUserDetail(x.MemberId)).ToList(),
                }).FirstOrDefault(),

                startDateTime = e.MatchDateTime.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                creator = _identityService.GetUserDetail(e.CreatedBy),
            }).Skip((validFilter.pageNumber.Value - 1) * validFilter.pageSize.Value)
                  .Take(validFilter.pageSize.Value)
                  .ToListAsync(cancellationToken);

            var totalRecords = await query.CountAsync(cancellationToken);
            return PaginationHelper.CreatePagedReponse(pagedData, validFilter, totalRecords);
        }
    }
}