using Application.Common.Interfaces;
using Domain.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public class MatchHub : Hub, IMatchHub
{
    private readonly IHubContext<MatchHub> _context;
    public MatchHub(IHubContext<MatchHub> context)
    {
        _context = context;
    }
    public async Task SendVerification(LocationVerificationContract verify)
    {
        await _context.Clients.All.SendAsync("LocationVerification", verify);
    }
}


