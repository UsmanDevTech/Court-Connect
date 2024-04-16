using Domain.Enum;

namespace Domain.Contracts;

public class ChatHeadContract
{
    public int chatHeadId { get; set; }
    public ChatTypeEnum type { get; set; }
    public int? matchId { get; set; }
    public string? matchTitle { get; set; }
    public string? matchPic { get; set; }
  
    public LastMessageContract? lastMsg { get; set; }
    public List<LeagueRankingUserContract> users { get; set; } = new();
}

public class LastMessageContract
{
    public int? messageId { get; set; }
    public string message { get; set; } = null!;
    public MediaTypeEnum Type { get; set; }
    public DateTime datetime { get; set; }
    public LeagueRankingUserContract sender { get; set; } = null!;
    public double? latitute { get; set; }
    public double? longitute { get; set; }
    public bool isSeen { get; set; }
}

public sealed class ChatHistory: ChatHeadContract
{
    public List<ConversationChat> ChatList { get; set; } = new();
    public List<GenericUserDetailContract> participants { get; set; } = new();
}

public class ConversationChat
{
    public int chatHeadId { get; set; }
    public int chatId { get; set; }
    public MediaTypeEnum messageType { get; set; }
    public string message { get; set; } = null!;
    public LeagueRankingUserContract sender { get; set; } = new();
    public DateTime sendTime { get; set; }
    public double? latitute { get; set; }
    public double? longitute { get; set; }
}
