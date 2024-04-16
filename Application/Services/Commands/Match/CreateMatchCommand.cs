using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Commands;

public sealed class CreateMatchCommand : IRequest<ResultContract>
{
    public int type { get; set; }
    public int matchCategory { get; set; }
    public string startTime { get; set; } = null!;
    public double latitute { get; set; }
    public double longitute { get; set; }
    public string address { get; set; } = null!;
    public string? thumbnail { get; set; }
    public string title { get; set; } = null!;
    public string? description { get; set; }
    public string? paymentIntent { get; set; }
    public double? amountPaid { get; set; }
}
internal sealed class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, ResultContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _dateTime;
    private readonly IMatchService _matchService;
    public IFormatProvider provider { get; private set; }
    public CreateMatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime, IIdentityService identityService, IMatchService matchService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _dateTime = dateTime;
        _matchService = matchService;
    }
    public async Task<ResultContract> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var timeZoneId = _identityService.GetTimezone(userId);
        if (userId == null)
            throw new NotFoundException("User not found");

        //Check for purchased packages
        SubscriptionHistory pck = null;

        if (request.paymentIntent == "")
        {
            if ((GameTypeEnum)request.matchCategory == GameTypeEnum.Unranked)
                pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Unranked);
            else
                pck = await _identityService.GetPackageDetailAsync(userId, GameTypeEnum.Ranked);
           
            if (pck == null)
                throw new CustomInvalidOperationException("Purchase subscription to create match!");
        }

        DateTime validTill;
        DateTime dateTime10; // 1/1/0001 12:00:00 AM  
        bool isSuccess = DateTime.TryParseExact(request.startTime, new string[] { "MM.dd.yyyy hh:mm tt", "MM-dd-yyyy hh:mm tt", "MM/dd/yyyy hh:mm tt" }, provider, DateTimeStyles.None, out dateTime10);
        if (isSuccess)
            validTill = DateTime.ParseExact(request.startTime, new string[] { "MM.dd.yyyy hh:mm tt", "MM-dd-yyyy hh:mm tt", "MM/dd/yyyy hh:mm tt" }, provider, DateTimeStyles.None);
        else
            throw new CustomInvalidOperationException("MM.dd.yyyy hh:mm tt, MM-dd-yyyy hh:mm tt, MM/dd/yyyy hh:mm tt these formats are supported only!");

        validTill = validTill.LocalToUtc(timeZoneId);

        if (string.IsNullOrEmpty(request.thumbnail))
            request.thumbnail = _matchService.BaseUrl() + "/Images/court-connect-logo.png";

        TennisMatch matchCreated = TennisMatch.Create(request.title, request.description,
             request.thumbnail, (MatchTypeEnum)request.type, (GameTypeEnum)request.matchCategory,
             request.latitute, request.longitute, request.address, validTill, MatchStatusEnum.Initial,
             false, _currentUser.UserId, _dateTime.NowUTC);

        await _context.TennisMatches.AddAsync(matchCreated);
        await _context.SaveChangesAsync(cancellationToken);

        ChatHead createdChatHead = ChatHead.Create(ChatTypeEnum.Group, matchCreated.Id, userId, _dateTime.NowUTC);
        createdChatHead.AddChatMember(userId, _dateTime.NowUTC);

        _context.ChatHeads.Add(createdChatHead);

        //await _chatHubService.SendChatHead(createdChatHead.Id, 1);

        if (request.paymentIntent == "")
        {
            //Update the remaining match
            if (request.type == (int)GameTypeEnum.Unranked)
            {
                if (pck.RemainingFreeUnrankedGames == 0)
                    pck.RemainingFreeUnrankedGames = 0;
                else
                    pck.RemainingFreeUnrankedGames -= 1;

                _context.SubscriptionHistories.Update(pck);
            }
            else if (request.type == (int)GameTypeEnum.Ranked)
            {
                if (pck.RemainingFreeRankedGames == 0)
                    pck.RemainingFreeRankedGames = 0;
                else
                    pck.RemainingFreeRankedGames -= 1;

                _context.SubscriptionHistories.Update(pck);
            }
        }
        else
        {
            matchCreated.AddMatchPurchase(userId, (double)request.amountPaid, 0.0, 0.0, request.paymentIntent, _dateTime.NowUTC);
        }

        //Create notification to same leage users

        var userLeague = await _context.UserSettings.Include(x => x.SubLeague).Where(x => x.UserId == userId).FirstOrDefaultAsync();
        var leagueUsers = await _context.UserSettings.Where(x => x.SubLeagueId == userLeague.SubLeagueId).Select(x => x.UserId).ToListAsync() ?? new List<string>();

        if (leagueUsers.Count() > 0)
        {
            var title = "New Match Available To Join";
            var description = matchCreated.Title + " is available to join with start date " + matchCreated.MatchDateTime.UtcToLocalTime(timeZoneId).ToString(_dateTime.longDayDateTimeFormat);

            foreach (var item in leagueUsers.Where(x => x != userId))
            {
                _context.Notifications.AddAsync(Notification.Create(null, title,
                    description, item, matchCreated.Id, NotificationTypeEnum.Clickable,
                    NotificationRedirectEnum.MatchDetail, matchCreated.MatchCategory, _dateTime.NowUTC, userId));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
      
        return new ResultContract { id = matchCreated.Id};
    }
}
