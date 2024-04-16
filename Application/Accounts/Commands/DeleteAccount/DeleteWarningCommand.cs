using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Accounts.Commands;

public sealed record DeleteWarningCommand(int id) : IRequest<Result>;
internal sealed class DeleteWarningCommandHandler : IRequestHandler<DeleteWarningCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteWarningCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteWarningCommand request, CancellationToken cancellationToken)
    {
        var warning = _context.ProfileWarnings.Where(x => x.Id == request.id).FirstOrDefault();
        if (warning == null)
            throw new NotFoundException("Warning Not Found");

        _context.ProfileWarnings.Remove(warning);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
