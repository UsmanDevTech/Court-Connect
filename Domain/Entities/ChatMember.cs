using Domain.Common;

namespace Domain.Entities;

public class ChatMember: BaseAuditableEntity
{
    internal ChatMember(string participantId,bool isChatHeadDeleted, int? lastMsgSeenId, int? chatHeadDeleteLastMsgId, string createdBy, DateTime created)
    {
        ParticipantId = participantId;
        IsChatHeadDeleted = isChatHeadDeleted;
        LastMsgSeenId = lastMsgSeenId;
        ChatHeadDeleteLastMsgId = chatHeadDeleteLastMsgId;
        CreatedBy = createdBy;
        Created = created;
    }
    public ChatMember(){}
    //Properties
    public string ParticipantId { get; set; } = null!;
    public bool IsChatHeadDeleted { get; set; }
   
    //Foreign Key
    public int ChatHeadId { get; private set; }
    public ChatHead ChatHead { get; private set; } = null!;

    public int? ChatHeadDeleteLastMsgId { get; private set; }
    public ChatConversation? ChatHeadDeleteLastMsg { get; private set; }

    public int? LastMsgSeenId { get; private set; }
    public ChatConversation? LastMsgSeen { get; private set; }

    //Methods
    public void setChatHeadObjectRefrence(ChatHead chatHead)
    {
        ChatHead = chatHead;
    }

    public void headDelete(int? chatHeadDeleteLastMsgId)
    {
        ChatHeadDeleteLastMsgId = chatHeadDeleteLastMsgId;
    }

    public void isChatHeadDelete(bool isDelete)
    {
        IsChatHeadDeleted = isDelete;
    }

    public void lastReadMessage(int msgId)
    {
        LastMsgSeenId = msgId;
    }
}
