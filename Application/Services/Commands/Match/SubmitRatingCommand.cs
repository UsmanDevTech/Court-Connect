using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;


public sealed class SubmitRatingCommand : IRequest<Result>
{
    public int matchId { get; set; }
    public RatingContract score { get; set; } = new();
}
internal sealed class SubmitRatingCommandHandler : IRequestHandler<SubmitRatingCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _datetimeService;
    private readonly IIdentityService _identityService;

    public SubmitRatingCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
         IDateTime datetimeService, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _datetimeService = datetimeService;
        _identityService = identityService;
    }
    public async Task<Result> Handle(SubmitRatingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var match = _context.TennisMatches.Include(x => x.MatchJoinRequests).Include(x => x.MatchMembers)
            .Where(x => x.Id == request.matchId).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match not found");

        var checkMember = match.MatchMembers.Where(x => x.MemberId == userId).FirstOrDefault();
        if (checkMember == null)
            throw new CustomInvalidOperationException("Only member can provide the rating!");

        //Check for member
        if (!match.MatchMembers.Where(x => x.TeamCode != checkMember.TeamCode).Select(x => x.MemberId).ToList().Contains(request.score.memberId))
            throw new CustomInvalidOperationException("You can only rate your opponent!");

        match.AddMatchReview(userId, request.score.memberId, request.score.forehead, request.score.backhand, request.score.serve, request.score.fairness, request.score.comment, _datetimeService.NowUTC);
        await _context.SaveChangesAsync(cancellationToken);

        //Update user profile rating
        var userRating = _context.MatchReviews.Where(x => x.ReviewedTo == request.score.memberId).ToList();
        if (userRating != null && userRating.Count() > 0)
        {
            var overallRating = 0.0;
            foreach (var item in userRating)
            {
                var matchRating = ((item.Fairness + item.Forehand + item.Backhand + item.Serve) / 20) * 5;
                overallRating += matchRating;
            }

            overallRating = overallRating / userRating.Count();
            await _identityService.UpdateRatingAsync(request.score.memberId, overallRating, userRating.Count());
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

