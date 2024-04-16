using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetReveiwByUserQuery(string userId) : IRequest<List<MatchRatingProfileContract>>;
internal sealed class GetReveiwByUserQueryHandler : IRequestHandler<GetReveiwByUserQuery, List<MatchRatingProfileContract>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTimeService;
    public GetReveiwByUserQueryHandler(IApplicationDbContext context, IIdentityService identityService,
        IDateTime dateTimeService, ICurrentUserService currentUserService)
    {
        _context = context;
        _identityService = identityService;
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    public async Task<List<MatchRatingProfileContract>> Handle(GetReveiwByUserQuery request, CancellationToken cancellationToken)
    {
        var timezoneId = _identityService.GetTimezone(_currentUserService.UserId);

        return await _context.MatchReviews.Include(x => x.TennisMatch).Where(x => x.ReviewedTo == request.userId).Select(x => new MatchRatingProfileContract
        {
            user = _identityService.GetUserDetail(_currentUserService.UserId),
            serve = x.Serve,
            backhand = x.Backhand,
            fairness = x.Fairness,
            forehead = x.Forehand,
            comment = x.Comment,
            totalRating = Math.Round(((x.Fairness + x.Forehand + x.Backhand + x.Serve) / 20) * 5, 2),
            created = x.Created.UtcToLocalTime(timezoneId).ToString(_dateTimeService.dayFormat),
        }).ToListAsync(cancellationToken);
    }
}