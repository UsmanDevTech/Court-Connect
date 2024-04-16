using Domain.Contracts;

namespace Application.Common.Interfaces;

public interface ILongPollingService
{
    Task SendMessage(ConversationChat message);
    Task<ConversationChat> GetMessage();
}