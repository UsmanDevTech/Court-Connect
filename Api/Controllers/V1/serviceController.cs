using Application.Accounts.Queries;
using Application.Common.Models;
using Application.Services.Commands;
using Application.Services.Queries;
using Domain.Contracts;
using Domain.Enum;
using Domain.Generics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

[ApiVersion("1.0")]
public class serviceController : BaseController
{
    /// <summary>
    /// GET REQUESTS


  
    [HttpGet(Routes.Service.get_leagues_information)]
    public async Task<List<LeagueSubLeagueDetailContract>> getLeagueInformationAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetLeagueInformationQuery(), token);
    }

    [HttpGet(Routes.Service.get_couching_categories)]
    public async Task<List<GenericIconInfoContract>> getCouchingCategoriesAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetCouchingHubCategoryQuery(), token);
    }

    [HttpGet(Routes.Service.get_league_ranking)]
    public async Task<LeagueRankingContract> getLeagueRankingAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetLeagueRankingQuery(), token);
    }

    /// <summary>
    /// POST REQUESTS

    [HttpPost(Routes.Service.get_couching_content)]
    public async Task<PaginationResponseBaseWithActionRequest<List<CouchingHubContract>>> getCouchingContentAsync([FromBody] GetCoachingHubPaginationQuery request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPost(Routes.Service.purchase_couching_hub)]
    public async Task<Result> purchaseCouchingHubAsync([FromBody] PurchaseCouchingHubCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    //Match detail
    [HttpPost(Routes.Service.get_matches)]
    public async Task<PaginationResponseBaseWithActionRequest<List<TennisMatchContract>>> getMatchDetailAsync([FromBody] GetTennisDetailPaginationQuery request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpGet(Routes.Service.get_match_detail)]
    public async Task<TennisMatchDtlsContract> getMatchDetailByIdAsync([FromQuery] int matchId, CancellationToken token)
    {
        return await Mediator.Send(new GetMatchDetailByIdQuery(matchId), token);
    }

    //Get created and joined matches
    [HttpGet(Routes.Service.get_created_joined_matches)]
    public async Task<TennisJoinedCreatedMatchContract> getCreatedJoinedMatchesAsync(CancellationToken token)
    {
        return await Mediator.Send(new GetCreatedJoinedMatchesQuery(), token);
    }

    [HttpGet(Routes.Service.get_requestable_member)]
    public async Task<List<MatchRequestUsersContract>> getRequestableMemberAsync([FromQuery] int matchId, CancellationToken token)
    {
        return await Mediator.Send(new GetMatchRequestAbleUserPaginationQuery(matchId) , token);
    }
    


    [HttpPost(Routes.Service.create_match)]
    public async Task<ResultContract> createMatchAsync([FromBody] CreateMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Service.update_match)]
    public async Task<Result> updateMatchAsync([FromBody] UpdateMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Service.cancel_match)]
    public async Task<Result> cancelMatchAsync([FromBody] CancelMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Service.leave_match)]
    public async Task<Result> leaveMatchAsync([FromBody] LeaveMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Service.start_match)]
    public async Task<Result> startMatchAsync([FromBody] StartMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }



    [HttpPost(Routes.Service.send_match_member_request)]
    public async Task<Result> sendMatchMemberRequestAsync([FromBody] SendMemberRequestCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    
    [HttpGet(Routes.Service.accept_reject_match_request)]
    public async Task<Result> acceptRejectMatchRequestAsync([FromQuery] int matchRequestId, StatusEnum status, string? paymentIntent, double? amountPaid, CancellationToken token)
    {
        return await Mediator.Send(new AcceptRejectMatchRequestCommand(matchRequestId, status, paymentIntent, amountPaid), token);
    }

    [HttpPost(Routes.Service.join_match)]
    public async Task<Result> joinMatchAsync([FromBody] JoinMatchCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    //New Apis

    [HttpPost(Routes.Service.send_match_score)]
    public async Task<Result> sendMatchScoreAsync([FromBody] SendMatchScoreCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpGet(Routes.Service.get_pre_final_score)]
    public async Task<ScoreConfirmationContract> getPreFinalScoreAsync([FromQuery] int matchId, CancellationToken token)
    {
        return await Mediator.Send(new GetPreFinalMatchScoreQuery(matchId), token);
    }

    [HttpPut(Routes.Service.approve_reject_score)]
    public async Task<Result> approveRejectScoreAsync([FromBody] ApproveOrRejectMatchScoreCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    
    [HttpGet(Routes.Service.verify_match_score_approval)]
    public async Task<Result> verifyMatchScoreAsync([FromQuery] int matchId, string winnerTeam, CancellationToken token)
    {
        return await Mediator.Send(new VerifyMatchScoreQuery(matchId, winnerTeam), token);
    }

    [HttpPost(Routes.Service.match_location_update)]
    public async Task<Result> matchLocationUpdateAsync([FromBody] UpdateLocationCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpGet(Routes.Service.get_final_score)]
    public async Task<ScoreCalculationContract> getFinalMatchScoreAsync([FromQuery] int matchId, CancellationToken token)
    {
        return await Mediator.Send(new GetMatchCalculationScoreQuery(matchId), token);
    }

   
    [HttpPost(Routes.Service.make_match_winner)]
    public async Task<Result> makeMatchWinnerAsync([FromBody] MakeMatchWinnerCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPost(Routes.Service.provide_feedback)]
    public async Task<Result> provideFeedbackAsync([FromBody] SubmitRatingCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpGet(Routes.Account.profile_feedback)]
    public async Task<MatchRatingContract> getProfileFeedBack([FromQuery] string? otherUser, CancellationToken token)
    {
        return await Mediator.Send(new GetProfileReviewQuery(otherUser), token);
    }

    [HttpPost(Routes.Account.profile_score)]
    public async Task<PaginationResponseBase<List<ProfileScoreContract>>> getProfileScore([FromBody] GetProfileScoreQuery request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }


    [HttpGet(Routes.Account.profile_match_balance)]
    public async Task<MatchBalanceContract> getProfileMatchBalanceAsync([FromQuery] string? otherUser, CancellationToken token)
    {
        return await Mediator.Send(new GetMatchBalanceQuery(otherUser), token);
    }

    [HttpGet(Routes.Account.profile_match_ranking)]
    public async Task<List<LineRankingChart>> getProfileMatchRankingAsync([FromQuery] string? otherUser, int duration ,CancellationToken token)
    {
        return await Mediator.Send(new GetRatingQuery(otherUser, duration ), token);
    }
}
