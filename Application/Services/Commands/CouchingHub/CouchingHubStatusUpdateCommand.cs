using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Subscriptions.Commands;

public sealed record CouchingHubStatusUpdateCommand(int id, bool isDeleted) : IRequest<Result>;
internal sealed class CouchingHubStatusUpdateCommandHandler : IRequestHandler<CouchingHubStatusUpdateCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public CouchingHubStatusUpdateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(CouchingHubStatusUpdateCommand request, CancellationToken cancellationToken)
    {
        var existingCouchingHub = await _context.CouchingHubs.FindAsync(request.id);

        if (existingCouchingHub == null)
            throw new NotFoundException("Couching hub not found");

        existingCouchingHub.Deleted = request.isDeleted;
        _context.CouchingHubs.Update(existingCouchingHub);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
}
