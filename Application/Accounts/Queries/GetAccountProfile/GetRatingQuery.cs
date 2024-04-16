
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;

namespace Application.Accounts.Queries;

public sealed record GetRatingQuery(string? otherUser, int duration) : IRequest<List<LineRankingChart>>;
internal sealed class GetRatingQueryHandler : IRequestHandler<GetRatingQuery, List<LineRankingChart>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    public GetRatingQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<List<LineRankingChart>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        DateTime s_date = DateTime.Now;
        DateTime Start_check_Date = DateTime.Now;
        DateTime End_check_Date = DateTime.Now;

        if (request.duration == 3)
            s_date = DateTime.Now.AddMonths(-3);
        else if (request.duration == 6)
            s_date = DateTime.Now.AddMonths(-6);
        else if (request.duration == 9)
            s_date = DateTime.Now.AddMonths(-9);
        else if (request.duration == 12)
            s_date = DateTime.Now.AddMonths(-12);

        List<LineRankingChart> graphList = new List<LineRankingChart>();

        if (request.otherUser != "" && request.otherUser != null)
        {
            for (int i = 1; i <= request.duration; i++)
            {
                var year = s_date.Year;
                var startMonth = s_date.Month;

                Start_check_Date = new DateTime(s_date.Year, s_date.Month, 1);
                End_check_Date = new DateTime(s_date.AddMonths(1).Year, s_date.AddMonths(1).Month, 1).AddMinutes(-1);

                var rating = _context.MatchReviews.Where(o => o.Created <= End_check_Date && o.Created >= Start_check_Date
                && o.ReviewedTo == request.otherUser).ToList();

                var overallRating = 0.0;
                if (rating != null && rating.Count() > 0)
                {
                    foreach (var item in rating)
                    {
                        var matchRating = ((item.Fairness + item.Forehand + item.Backhand + item.Serve) / 20) * 5;
                        overallRating += matchRating;
                    }

                    overallRating = overallRating / rating.Count();
                }
                var graph = new LineRankingChart
                {
                    year = year,
                    MonthNumber = startMonth,
                    month = _identityService.GetMonthName(startMonth),
                    rating = overallRating,
                };

                graphList.Add(graph);

                s_date = s_date.AddMonths(1);
            }

            return graphList;
        }
        else
        {
            for (int i = 1; i <= request.duration; i++)
            {
                var overallRating = 0.0;

                var year = s_date.Year;
                var startMonth = s_date.Month;

                Start_check_Date = new DateTime(s_date.Year, s_date.Month, 1);
                End_check_Date = new DateTime(s_date.AddMonths(1).Year, s_date.AddMonths(1).Month, 1).AddMinutes(-1);

                var rating = _context.MatchReviews.Where(o => o.Created <= End_check_Date && o.Created >= Start_check_Date
                && o.ReviewedTo == userId).ToList();

                if (rating != null && rating.Count() > 0)
                {
                    foreach (var item in rating)
                    {
                        var matchRating = ((item.Fairness + item.Forehand + item.Backhand + item.Serve) / 20) * 5;
                        overallRating += matchRating;
                    }

                    overallRating = overallRating / rating.Count();
                }
                var graph = new LineRankingChart
                {
                    year = year,
                    MonthNumber = startMonth,
                    month = _identityService.GetMonthName(startMonth),
                    rating = Math.Round(overallRating, 2),
                };

                graphList.Add(graph);

                s_date = s_date.AddMonths(1);
            }

            return graphList;
        }
    }
}