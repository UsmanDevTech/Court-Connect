
using Domain.Contracts;

namespace Application.Common.Interfaces;

public interface IChatHub
{
    Task SendMessage(ConversationChat chat);
    Task SendVerification(LocationVerificationContract verify);
}
