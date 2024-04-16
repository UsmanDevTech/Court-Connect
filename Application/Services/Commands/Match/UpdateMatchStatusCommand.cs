using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Services.Commands.Match;

public sealed record UpdateMatchStatusCommand(int id, bool isDeleted, string? reason) : IRequest<Result>;
internal sealed class UpdateMatchStatusCommandHandler : IRequestHandler<UpdateMatchStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
   
    public UpdateMatchStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateMatchStatusCommand request, CancellationToken cancellationToken)
    {
        var match = _context.TennisMatches.Where(x => x.Id == request.id).FirstOrDefault();
        if (match == null)
            throw new NotFoundException("Match Not Found");

        if (request.isDeleted == true)
        {
            match.Deleted = false;
            _context.TennisMatches.Update(match);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            match.Deleted = true;
            match.BlockReason = request.reason;
            _context.TennisMatches.Update(match);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}
