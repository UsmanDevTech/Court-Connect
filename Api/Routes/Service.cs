namespace Api.Routes;

public class Service
{
    //Subscription
    public const string get_subscription = "getSubscription";
    public const string purchase_subscription = "purchaseSubscription";
    public const string unsubscribe_subscription = "unsubscribeSubscription";

    //Couching Hub
    public const string purchase_couching_hub = "purchaseCouchingHub";
    public const string get_couching_content = "getCouchingHub";
    public const string get_couching_categories = "getCouchingCategories";
  
    //League
    public const string get_leagues_information = "getLeaguesInformation";
    public const string get_league_ranking = "getLeagueRanking";

    //Match
    public const string create_match = "createMatch";
    public const string update_match = "updateMatch";
    public const string cancel_match = "cancelMatch";
    public const string leave_match = "leaveMatch";
    public const string start_match = "startMatch";

    public const string get_matches = "getMatches";
    public const string get_match_detail = "getMatchDetail";
    public const string get_created_joined_matches = "getCreatedJoinedMatches";
    public const string get_pre_final_score = "getPreFinalScore";
    public const string get_final_score = "getFinalScore";

    public const string get_requestable_member = "getRequestableMember";
    public const string send_match_member_request = "sendMatchMemberRequest";
    public const string accept_reject_match_request = "acceptRejectMatchRequest";
    public const string join_match = "joinMatch";
    public const string approve_reject_score = "approveRejectScore";
    public const string verify_match_score_approval = "verifyMatchScoreApproval";
    public const string make_match_winner = "makeMatchWinner";
    public const string send_match_score = "sendMatchScore";
    public const string provide_feedback = "provideFeedback";
    public const string match_location_update = "matchLocationUpdate";

    
}
