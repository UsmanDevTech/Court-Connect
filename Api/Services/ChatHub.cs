using Application.Common.Interfaces;
using Domain.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class ChatHub : Hub, IChatHub
{
    private readonly IHubContext<ChatHub> _context;
    public ChatHub(IHubContext<ChatHub> context)
    {
        _context = context;
    }
    public async Task SendMessage(ConversationChat chat)
    {
        await _context.Clients.All.SendAsync("ReceiveMessage", chat);
    }
    public async Task SendVerification(LocationVerificationContract verify)
    {
        await _context.Clients.All.SendAsync("LocationVerification", verify);
    }
}


