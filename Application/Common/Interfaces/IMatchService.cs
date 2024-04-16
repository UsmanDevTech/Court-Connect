
namespace Application.Common.Interfaces;

public interface IMatchService
{
    string CalculateNewEloRating(double player_a_elo, double player_b_elo, List<string> final_score, string? tie_break,
        int games_played_player_a, int games_played_player_b);
    string BaseUrl();
}