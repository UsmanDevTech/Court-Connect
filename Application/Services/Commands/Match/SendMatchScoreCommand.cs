using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed class SendMatchScoreCommand : IRequest<Result>
{
    public int matchId { get; set; }
    public int teamAScoreOne { get; set; }
    public int teamAScoreTwo { get; set; }
    public int teamAScoreThree { get; set; }
    public int teamBScoreOne { get; set; }
    public int teamBScoreTwo { get; set; }
    public int teamBScoreThree { get; set; }
}
internal sealed class SendMatchScoreCommandHandler : IRequestHandler<SendMatchScoreCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    public IFormatProvider provider { get; private set; }
    public SendMatchScoreCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(SendMatchScoreCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var tennisMatch = _context.TennisMatches.Include(x => x.MatchMembers)
            .Include(x => x.TemporaryMatchScores).Where(x => x.Id == request.matchId).FirstOrDefault();
        if (tennisMatch == null)
            throw new NotFoundException("Match not found");

        var checkOwner = tennisMatch.MatchMembers.Where(x => x.CreatedBy == userId).FirstOrDefault();
        if (checkOwner == null)
            throw new CustomInvalidOperationException("Only owner can send score verification request!");


        //Remove previous temp score
        var existingScore = tennisMatch.TemporaryMatchScores.FirstOrDefault();
        if (existingScore != null)
            tennisMatch.RemoveTempScore(existingScore);

        //Create new temp score
        await _context.TemporaryMatchScores.AddAsync(Domain.Entities.TemporaryMatchScore.Create(
            request.teamAScoreOne, request.teamAScoreTwo, request.teamAScoreThree,
            request.teamBScoreOne, request.teamBScoreTwo, request.teamBScoreThree, request.matchId));

        var title = "Score verification for match " + tennisMatch.Title;
        var description = "Score has been updated by the match creator. Please verify it!";


        //Clear previous notification
        var previousNotification = _context.Notifications
            .Where(x => x.RedirectId == tennisMatch.Id && x.RedirectType == NotificationRedirectEnum.ScoreVerification).ToList();

        if (previousNotification != null && previousNotification.Count() > 0)
        {
            foreach (var item in previousNotification)
            {
                _context.Notifications.Remove(item);
            }
        }

        //Create new notification
        foreach (var item in tennisMatch.MatchMembers.Where(x => x.MemberId != x.CreatedBy).ToList())
        {
            item.updateIsScored(StatusEnum.Pending);
            _context.Notifications.AddAsync(Domain.Entities.Notification.Create(null, title,
                description, item.MemberId, tennisMatch.Id, NotificationTypeEnum.Clickable,
                NotificationRedirectEnum.ScoreVerification, tennisMatch.MatchCategory, _dateTime.NowUTC, userId));

            _context.MatchMembers.Update(item);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
