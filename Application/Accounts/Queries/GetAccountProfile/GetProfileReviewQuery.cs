using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;

namespace Application.Accounts.Queries;

public sealed record GetProfileReviewQuery(string? otherUser) : IRequest<MatchRatingContract>;
internal sealed class GetProfileReviewQueryHandler : IRequestHandler<GetProfileReviewQuery, MatchRatingContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
    public GetProfileReviewQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime datetime, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetime = datetime;
        _identityService = identityService;
    }

    public async Task<MatchRatingContract> Handle(GetProfileReviewQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timezoneId = _identityService.GetTimezone(userId);


        if (request.otherUser != "" && request.otherUser != null)
        {
            return  new MatchRatingContract
            {
                overallRating = _identityService.GetUserRating(request.otherUser),
                reviewsCount = _context.MatchReviews.Where(x => x.ReviewedTo == request.otherUser).ToList().Count(),
                ratings = _context.MatchReviews.Where(x => x.ReviewedTo == request.otherUser).Select(e => new MatchRatingProfileContract
                {
                    totalRating = Math.Round(((e.Fairness + e.Forehand + e.Backhand + e.Serve) / 20) * 5, 2),
                    fairness = e.Fairness,
                    forehead = e.Forehand,
                    backhand = e.Backhand,
                    comment = e.Comment,
                    serve = e.Serve,
                    created = e.Created.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                    user = _identityService.GetUserDetail(e.CreatedBy),
                }).ToList(),
            };
        }
        else
        {
            return new MatchRatingContract
            {
                overallRating = _identityService.GetUserRating(userId),
                reviewsCount = _context.MatchReviews.Where(x=>x.ReviewedTo == userId).ToList().Count(),
                ratings = _context.MatchReviews.Where(x => x.ReviewedTo == userId).Select(e => new MatchRatingProfileContract
                {
                    totalRating = Math.Round(((e.Fairness + e.Forehand + e.Backhand + e.Serve) / 20) * 5, 2),
                    fairness = e.Fairness,
                    forehead = e.Forehand,
                    backhand = e.Backhand,
                    comment = e.Comment,
                    serve = e.Serve,
                    created = e.Created.UtcToLocalTime(timezoneId).ToString(_datetime.longDayDateFormat),
                    user = _identityService.GetUserDetail(e.CreatedBy),
                }).ToList(),
            };
        }
    }
}