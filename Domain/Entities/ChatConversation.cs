
using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;

public class ChatConversation: BaseAuditableEntity
{
    internal ChatConversation(string senderId, bool readStatus, double? latitute, double? longitue,
        string message, MediaTypeEnum messageType, string createdBy, DateTime created)
    {
        SenderId = senderId;
        ReadStatus = readStatus;
        Latitute = latitute;
        Longitue = longitue;
        Message = message;
        MessageType = messageType;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
  
    public string SenderId { get; set; } = null!;
    public bool ReadStatus { get; set; }
    public double? Latitute { get; set; }
    public double? Longitue { get; set; }
    public string Message { get; set; }
    public MediaTypeEnum MessageType { get; set; }

    //Foreign Key
    public int ChatHeadId { get; private set; }
    public ChatHead ChatHead { get; private set; } = null!;


    /// <summary>
    /// Last message at chat head delete time
    /// </summary>
    private readonly List<ChatMember> _chatHeadDeleteLastMsgs = new();
    public IReadOnlyCollection<ChatMember> ChatHeadDeleteLastMsgs => _chatHeadDeleteLastMsgs.AsReadOnly();

    /// <summary>
    /// Chat last message seen
    /// </summary>
    private readonly List<ChatMember> _lastMsgSeens = new();
    public IReadOnlyCollection<ChatMember> LastMsgSeens => _lastMsgSeens.AsReadOnly();


    //Methods
    public void setChatHeadObjectRefrence(ChatHead chatHead)
    {
        ChatHead = chatHead;
    }
}
