using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Subscriptions.Commands;

public sealed record SubscriptionStatusUpdateCommand(int id, bool isDeleted) : IRequest<Result>;
internal sealed class DeleteLanguageCommandHandler : IRequestHandler<SubscriptionStatusUpdateCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public DeleteLanguageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(SubscriptionStatusUpdateCommand request, CancellationToken cancellationToken)
    {
        var existingSubscription = await _context.Subscriptions.FindAsync(request.id);

        if (existingSubscription == null)
            throw new NotFoundException("Subscription not found");

        existingSubscription.Deleted = request.isDeleted;
        _context.Subscriptions.Update(existingSubscription);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
}
