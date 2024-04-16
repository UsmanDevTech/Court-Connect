
using Application.Accounts.Commands;
using Application.Accounts.Queries;
using Application.Common.Models;
using Application.Services.Queries;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enum;
using Domain.Generics;
using NetTopologySuite.Geometries;

namespace Application.Common.Interfaces;

public interface IIdentityService
{

    /// <summary>
    /// Queries
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// 

    Task<bool> AddTestingUserData(ttt request, CancellationToken token);

    Task<bool> BeUniqueEmailAsync(string email, CancellationToken token);
    Task<bool> BeUniquePhoneAsync(string phone, CancellationToken token);
    Task<SubscriptionHistory> GetPackageDetailAsync(string userId, GameTypeEnum type);
    Task<SubscriptionHistory> GetRefundPackageDetailAsync(string userId);

    Point GetLocation(string userId);
    bool VerifyUser(string userId);
    double GetUserRating(string userId);
    string GetUserName(string userId);
    string GetTimezone(string userId);
    int GetUserGender(string userId);
    int GetUserPoints(string userId);
    int GetUserLevel(string userId);
    string GetTeamCode(string userId, int matchId);
    public LeagueRankingUserContract GetUserDetail(string userId);
    public List<MatchRequestUsersContract> GetMatchRequestUserDetail(int matchId, string createdBy);
    public Task<ResponseKeyContract> AuthenticateUserAsync(LoginQuery request);
    public Task<UserProfileInfoDetailContract> GetAccountProfileAsync(GetAccountProfileQuery query);
    public Task<LeagueRankingContract> GetLeagueRanking(CancellationToken token);
    //public Task<List<LeagueRankingUserContract>> GetRankingBySubLeague(CancellationToken token);
    public Task<Result> ResetPasswordAsync(string password);
    public Task<PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>> GetTennisMatches(GetTennisDetailPaginationQuery request, CancellationToken token);
    public Task<DatatableResponse<List<UserProfileInfoDetailStatusContract>>> GetAllUsersDetailAsync(DataTablePaginationFilter filter, CancellationToken token);
    public Task<List<MatchRequestUsersContract>> GetMatchRequestAbleUsers(GetMatchRequestAbleUserPaginationQuery request, CancellationToken token);
    public Task<int> GetUsersCounterAsync();
    public Task<List<CouchingHubPurchasedHistory>> GetPurchasingUserDetailAsync(GetHubPurchasingUsersQuery request, CancellationToken token);

    public Task<DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>> GetMatchCreatorDetailAsync(DataTablePaginationFilter filter, CancellationToken token);

    /// <summary>
    /// Commands
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// 

    public Task<(Result Result, string UserId)> CreateAccountAsync(CreateAccountCommand request, CancellationToken cancellationToken);
    public Task<Result> SendEmailOTPAsync(SendEmailOtpCommand request, CancellationToken token);
    public Task<ResponseKeyContract> ConfirmEmailAsync(ConfirmEmailCommand request, CancellationToken cancellationToken);
    public Task<(Result Result, string UserId)> ResetPasswordViaEmailAsync(ResetPasswordViaEmailCommand request);
    public Task<Result> RequestForAccountDeleteAsync(string password);
    public Task<Result> RemoveProfileImageAsync();
    Task<Result> UpdateUserPackageDetailAsync(string userId);
    public Task<Result> UpdateAccountDetailAsync(UpdateProfileCommand request, CancellationToken cancellationToken);
    public Task<Result> LogoutAsync();
    public Task<Result> UpdateUserStatusAsync(StatusUpdateCommand request);
    public Task<Result> UpdateRatingAsync(string userId, double rating, int reviewBy);
    
    Task<Result> UpdateUserPointAsync(string user, double point);

    //Stripe Configuration
    Task<string> PayByStripeSubscriptionAsync(int subscriptionId, string userId, string baseUrl, string success, string cancel, string failed, CancellationToken token);

    Task<string> OneTimeStripePaymentAsync(double amount, string userId, string baseUrl, string success, string cancel, string failed, CancellationToken token);

    string GetMonthName(int month);


    //Admin Match Detail
    //public Task<DatatableResponse<List<GenericPostQueryListModel<MatchOwnerDetailContract>>>> GetMatchCreatorDetailAsync(DataTablePaginationFilter filter, CancellationToken token);

}

