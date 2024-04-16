using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed record ReadUserAllNotificationsCommand : IRequest<Result>;
internal sealed class ReadUserAllNotificationsCommandHandler : IRequestHandler<ReadUserAllNotificationsCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public ReadUserAllNotificationsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ReadUserAllNotificationsCommand request, CancellationToken cancellationToken)
    {

        var notifications =await _context.Notifications.Where(u => u.NotifyTo == _currentUser.UserId && u.IsSeen==false).ToListAsync();
        notifications.ForEach(a => a.UpdateReadStatus(true));

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
