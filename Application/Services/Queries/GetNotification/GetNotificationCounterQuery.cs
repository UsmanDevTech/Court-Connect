using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public sealed record GetNotificationCounterQuery:IRequest<NotificationCounterContract>;

public sealed class GetNotificationCounterQueryHandler : IRequestHandler<GetNotificationCounterQuery, NotificationCounterContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationCounterQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<NotificationCounterContract> Handle(GetNotificationCounterQuery request, CancellationToken cancellationToken)
    {
        return new NotificationCounterContract(await _context.Notifications.Where(u => u.NotifyTo == _currentUser.UserId && u.IsSeen == false).CountAsync(cancellationToken));
    }
}