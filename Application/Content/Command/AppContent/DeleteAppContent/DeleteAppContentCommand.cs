using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Content.Command;

public sealed record DeleteAppContentCommand(int id) : IRequest<Result>;
internal sealed class DeleteAppContentCommandHandler : IRequestHandler<DeleteAppContentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public DeleteAppContentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(DeleteAppContentCommand request, CancellationToken cancellationToken)
    {
        var existingSetting = await _context.AppContent.FindAsync(request.id);

        if (existingSetting == null)
            throw new NotFoundException("Invalid Id");

        _context.AppContent.Remove(existingSetting);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}