using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Command.Notification;

public sealed record ReadSingleNotificationCommand(int id) : IRequest<Result>;
internal sealed class ReadSingleNotificationCommandHandler : IRequestHandler<ReadSingleNotificationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public ReadSingleNotificationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(ReadSingleNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications.Where(u => u.Id == request.id).FirstOrDefaultAsync(cancellationToken);
        if (notification == null)
            return Result.Success();

        notification.UpdateReadStatus(true);
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}