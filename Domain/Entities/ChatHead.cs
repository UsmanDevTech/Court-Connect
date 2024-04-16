using Domain.Abstraction;
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class ChatHead : BaseAuditableEntity, ISoftDelete
{
    private ChatHead(ChatTypeEnum type, int? tennisMatchId,
       bool deleted, string createdBy, DateTime created)
    {
        Type = type;
        TennisMatchId = tennisMatchId;
        Deleted = deleted;
        CreatedBy = createdBy;
        LastMessageTime = DateTime.UtcNow;
        Created = created;
    }

    //Properties

    public ChatTypeEnum Type { get; set; }

    //Foreign key
    public int? TennisMatchId { get; set; }
    public TennisMatch? TennisMatch { get; set; }

    public DateTime LastMessageTime { get; set; }

    private bool _deleted;
    public bool Deleted
    {
        get => _deleted;
        set
        {
            if (value == true && _deleted == false)
            {
                //Trigger Domain Event if any 
            }

            _deleted = value;
        }
    }

    //Foreign key

    /// <summary>
    /// Chat Members
    /// </summary>
    private readonly List<ChatMember> _chatMembers = new();
    public IReadOnlyCollection<ChatMember> ChatMembers => _chatMembers.AsReadOnly();

    /// <summary>
    /// Chat Conversation
    /// </summary>
    private readonly List<ChatConversation> _chatConversations = new();
    public IReadOnlyCollection<ChatConversation> ChatConversations => _chatConversations.AsReadOnly();

    //Factory Method
    public static ChatHead Create(ChatTypeEnum type, int? tennisMatchId,
       string createdBy, DateTime created)
    {
        return new ChatHead(type, tennisMatchId, false, createdBy, created);
    }

    public void AddChatMember(string participantId, DateTime createdAt)
    {
        var member = new ChatMember(participantId, false, null, null, null, createdAt);
        member.setChatHeadObjectRefrence(this);
        //Add Chat Member
        _chatMembers.Add(member);
    }

    public ChatConversation SendMessage(string createdBy, double? latitute, double? longitute, string? message,
        MediaTypeEnum type, DateTime createdAt)
    {
        var chat = new ChatConversation(createdBy, false, latitute, longitute, message, type, createdBy, createdAt);
        chat.setChatHeadObjectRefrence(this);
        //Add Chat Member
        return chat;
    }

    public void setMessageTime(DateTime lastMessageTime) 
    {
        LastMessageTime = lastMessageTime;
    }

    public void RemoveChatMember(ChatMember member)
    {
        _chatMembers.Remove(member);
    }
}
