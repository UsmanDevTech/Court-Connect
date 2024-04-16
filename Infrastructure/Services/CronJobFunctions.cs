using Application.Common.Interfaces;
using Domain.Enum;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class CronJobFunctions : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CronJobFunctions(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                using var scope = _serviceProvider.CreateScope();
                ApplicationDbContext _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                UserManager<ApplicationUser> _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                IIdentityService _identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

                DateTime date = DateTime.UtcNow;
                var hour = date.Hour;
                var minutes = date.Minute;

                var currentDay = DateTime.UtcNow.DayOfWeek;

                //Mark match waiting
                var matches = _context.TennisMatches.Include(x => x.MatchMembers).Where(e => e.MatchDateTime.Date <= date.Date && e.Status == MatchStatusEnum.Initial).ToList();
                if (matches != null && matches.Count > 0)
                {
                    foreach (var match in matches)
                    {
                        var matchDate = match.MatchDateTime - new TimeSpan(0, 15, 0);

                        if (matchDate.Date < date.Date)
                        {
                            if ((match.Type == MatchTypeEnum.Double || match.Type == MatchTypeEnum.Mixed) && match.MatchMembers.Count() < 4)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else if (match.Type == MatchTypeEnum.Single && match.MatchMembers.Count() < 2)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else
                            {
                                match.updateMatchStatus(MatchStatusEnum.ReadyToStart);
                            }

                            _context.TennisMatches.Update(match);
                        }
                        else if (matchDate.Date == date.Date && matchDate.Hour < hour)
                        {
                            if ((match.Type == MatchTypeEnum.Double || match.Type == MatchTypeEnum.Mixed) && match.MatchMembers.Count() < 4)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else if (match.Type == MatchTypeEnum.Single && match.MatchMembers.Count() < 2)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else
                            {
                                match.updateMatchStatus(MatchStatusEnum.ReadyToStart);
                            }

                            _context.TennisMatches.Update(match);
                        }
                        else if (matchDate.Date == date.Date && matchDate.Hour == hour && matchDate.Minute <= minutes)
                        {
                            if ((match.Type == MatchTypeEnum.Double || match.Type == MatchTypeEnum.Mixed) && match.MatchMembers.Count() < 4)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else if (match.Type == MatchTypeEnum.Single && match.MatchMembers.Count() < 2)
                            {
                                //Refund match to participants
                                foreach (var item in match.MatchMembers.ToList())
                                {
                                    if (match.MatchCategory == GameTypeEnum.Unranked)
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(0);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                    else
                                    {
                                        var pck = await _identityService.GetRefundPackageDetailAsync(item.MemberId);
                                        pck.UpdateRemainingCount(1);

                                        _context.SubscriptionHistories.Update(pck);
                                    }
                                }
                                match.updateMatchStatus(MatchStatusEnum.Expired);
                            }
                            else
                            {
                                match.updateMatchStatus(MatchStatusEnum.ReadyToStart);
                            }


                            _context.TennisMatches.Update(match);
                        }
                    }
                }

                // Expire subscription
                var subscriptionExpiry = _context.SubscriptionHistories.Include(x => x.Subscription).Where(e => e.ExpireAt.Date <= date.Date && e.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel
                && e.SubscriptionStatus != UserPackageStatusEnum.Expired).ToList();

                if (subscriptionExpiry.Count() > 0)
                {
                    foreach (var expiry in subscriptionExpiry)
                    {
                        if (expiry.ExpireAt.Date < date.Date)
                        {
                            expiry.UpdateStatus(UserPackageStatusEnum.Expired);
                            _context.SubscriptionHistories.Update(expiry);
                        }

                        else if (expiry.ExpireAt.Date == date.Date && expiry.ExpireAt.Hour < hour)
                        {
                            expiry.UpdateStatus(UserPackageStatusEnum.Expired);
                            _context.SubscriptionHistories.Update(expiry);
                        }
                        else if (expiry.ExpireAt.Date == date.Date && expiry.ExpireAt.Hour == hour && expiry.ExpireAt.Minute <= minutes)
                        {
                            expiry.UpdateStatus(UserPackageStatusEnum.Expired);
                            _context.SubscriptionHistories.Update(expiry);
                        }

                    }

                }

                ////User subscription status update
                var users = _userManager.Users.Where(e => e.isSubscriptionPurchased == true && e.LoginRole == UserTypeEnum.User).ToList() ?? new List<ApplicationUser>();
                if (users.Count() > 0)
                {
                    foreach (var user in users)
                    {
                        var activePackage = _context.SubscriptionHistories.Include(x => x.Subscription).Where(e => e.SubscriptionStatus == UserPackageStatusEnum.Active && e.Subscription.SubscriptionType != SubscriptionTypeEnum.FreemiumModel && e.CreatedBy == user.Id).FirstOrDefault();
                        if (activePackage == null)
                        {
                            user.isSubscriptionPurchased = false;
                            await _userManager.UpdateAsync(user);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

