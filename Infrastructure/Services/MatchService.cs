using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class MatchService : IMatchService
{
    private readonly IHttpContextAccessor _contextVal;

    [Obsolete]
    public MatchService(IHttpContextAccessor accessor)
    {
        _contextVal = accessor;
    }

    public string BaseUrl()
    {
        var request = _contextVal.HttpContext.Request;

        // Now that you have the request you can select what you need from it.
        var url = request.Scheme + "://" + request.Host.Value;
        return url;
    }

    public string CalculateNewEloRating(double player_a_elo, double player_b_elo, List<string> final_score, string? tie_break, int games_played_player_a, int games_played_player_b)
    {
        //Step 1: Calculate expected score of each player
        var player_a_expected = 1 / (1 + Math.Pow(10, (player_b_elo - player_a_elo) / 400));
        var player_b_expected = 1 / (1 + Math.Pow(10, (player_a_elo - player_b_elo) / 400));


        //# Step 2: Calculate actual score of each player
        var player_a_sets_won = 0;
        var player_b_sets_won = 0;

        var player_a_games_won = 0;
        var player_b_games_won = 0;

        var player_a_tiebreak_points = 0;
        var player_b_tiebreak_points = 0;


        foreach (var item in final_score)
        {
            string[] set_score = item.Split(":");

            var player_a_games = Convert.ToInt32(set_score[0]);
            var player_b_games = Convert.ToInt32(set_score[1]);



            if ((player_a_games == 6 && player_b_games <= 5) || (player_a_games == 7 && player_b_games <= 6))
            {
                player_a_sets_won += 1;
                player_a_games_won += player_a_games;
            }

            else if ((player_b_games == 6 && player_a_games <= 5) || (player_b_games == 7 && player_a_games <= 6))
            {
                player_b_sets_won += 1;
                player_b_games_won += player_b_games;
            }
            else if (player_a_games >= 10 && (player_a_games - player_b_games) >= 2)
            {
                player_a_sets_won += 1;
                player_a_games_won += player_a_games;
            }
            else if (player_b_games >= 10 && (player_b_games - player_a_games) >= 2)
            {
                player_b_sets_won += 1;
                player_b_games_won += player_b_games;
            }
            else if (player_a_games == 6 && player_b_games == 6) { }
            else
            {
                throw new CustomInvalidOperationException("Invalid score set!");
            }
        }

        if (tie_break != "")
        {
            string[] tie_set_score = tie_break.Split(":");

            player_a_tiebreak_points = Convert.ToInt32(tie_set_score[0]);
            player_b_tiebreak_points = Convert.ToInt32(tie_set_score[1]);
        }

        //# Calculate the performance of each player in terms of games won per set
        var player_a_performance = (player_a_games_won + (0.25 * player_a_tiebreak_points)) / ((player_a_games_won + player_b_games_won) + (0.25 * (player_a_tiebreak_points + player_b_tiebreak_points)));
        var player_b_performance = (player_b_games_won + (0.25 * player_b_tiebreak_points)) / ((player_a_games_won + player_b_games_won) + (0.25 * (player_a_tiebreak_points + player_b_tiebreak_points)));

        ////# Assign Groups to win and loss bonus
        //var k_factor = 0.0;

        ////# Step 3: Calculate new Elo rating for each player
        ////# THIS WILL BE CHANGED AFTER TESTING!

        //if ((player_a_elo + player_b_elo) / 2 < 600)
        //    k_factor = 64;
        //else if ((player_a_elo + player_b_elo) / 2 < 1000)
        //    k_factor = 42;
        //else if ((player_a_elo + player_b_elo) / 2 < 1500)
        //    k_factor = 32;
        //else if ((player_a_elo + player_b_elo) / 2 < 2000)
        //    k_factor = 16;
        //else if ((player_a_elo + player_b_elo) / 2 < 2400)
        //    k_factor = 10;


        //# Calculate new Elo rating for each player based on their performance

        if (player_a_sets_won > player_b_sets_won)
        {
            var win_bonus = win_bonus_match(player_a_performance);
            var loss_bonus = loss_bonus_match(player_b_performance);

            var player_a_new_elo = player_a_elo + (get_k_factor(player_a_elo, games_played_player_a) * (1 - player_a_expected) * (1.0 + win_bonus));
            var player_b_new_elo = player_b_elo + (get_k_factor(player_b_elo, games_played_player_b) * (0 - player_b_expected) * (1.0 + loss_bonus));

            player_a_new_elo = player_a_new_elo - player_a_elo;
            player_b_new_elo = player_b_elo - player_b_new_elo;

            return Convert.ToString(player_a_new_elo) + ":" + Convert.ToString(player_b_new_elo);
        }
        else
        {
            var win_bonus = win_bonus_match(player_b_performance);
            var loss_bonus = loss_bonus_match(player_a_performance);

            var player_a_new_elo = player_a_elo + (get_k_factor(player_a_elo, games_played_player_a) * (0 - player_a_expected) * (1.0 + loss_bonus));
            var player_b_new_elo = player_b_elo + (get_k_factor(player_b_elo, games_played_player_b) * (1 - player_b_expected) * (1.0 + win_bonus));

            player_a_new_elo = player_a_elo - player_a_new_elo;
            player_b_new_elo = player_b_new_elo - player_b_elo;

            return Convert.ToString(player_a_new_elo) + ":" + Convert.ToString(player_b_new_elo);
        }
        //return "2:2";
    }

    private double loss_bonus_match(double performance)
    {
        if (performance >= 0.45)
            return 0.5;
        else if (0.45 > performance && performance >= 0.4)
            return 0.4;
        else if (0.4 > performance && performance >= 0.35)
            return 0.3;
        else if (0.35 > performance && performance >= 0.3)
            return 0.2;
        else
            return 0.0;
    }

    private double win_bonus_match(double performance)
    {
        if (performance >= 0.95)
            return 0.5;
        else if (0.95 > performance && performance >= 0.9)
            return 0.4;
        else if (0.9 > performance && performance >= 0.8)
            return 0.3;
        else if (0.8 > performance && performance >= 0.7)
            return 0.2;
        else
            return 0.0;
    }

    private int get_k_factor(double players_points, int games_played)
    {
        if (players_points < 300)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 300 && players_points < 600)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 600 && players_points < 900)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 900 && players_points < 1200)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 1200 && players_points < 1500)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 1500 && players_points < 1800)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        else if (players_points >= 1800)
        {
            if (games_played < 5)
                return 64;
            else if (games_played < 10 && games_played >= 5)
                return 48;
            else if (games_played < 20 && games_played >= 10)
                return 32;
            else if (games_played >= 20)
                return 16;
        }
        
        return 0;
    }

}
