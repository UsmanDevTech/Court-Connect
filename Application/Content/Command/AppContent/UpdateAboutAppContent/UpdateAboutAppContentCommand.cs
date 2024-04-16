using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Services.Command;

public sealed record UpdateAboutAppContentCommand(int id, string name, string value, string? icon) : IRequest<Result>;
internal sealed class UpdateAboutAppContentCommandHandler : IRequestHandler<UpdateAboutAppContentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public UpdateAboutAppContentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(UpdateAboutAppContentCommand request, CancellationToken cancellationToken)
    {
        var existingSetting = await _context.AppContent.FindAsync(request.id);
        
        if (existingSetting == null)
            throw new NotFoundException("Invalid Id");

        if (!string.IsNullOrEmpty(request.name))
            existingSetting.UpdateName(request.name);

        if (!string.IsNullOrEmpty(request.value))
            existingSetting.UpdateValue(request.value);

        if (!string.IsNullOrEmpty(request.icon))
            existingSetting.UpdateIcon(request.icon);

        existingSetting.Modified = DateTime.UtcNow;

        _context.AppContent.Update(existingSetting);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}