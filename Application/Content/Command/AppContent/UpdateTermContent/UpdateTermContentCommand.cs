using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Command;

public sealed record UpdateTermContentCommand(int type, string htmlContent) : IRequest<Result>;
internal sealed class UpdateTermContentCommandHandler : IRequestHandler<UpdateTermContentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public UpdateTermContentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(UpdateTermContentCommand request, CancellationToken cancellationToken)
    {
        var existingAppContent = await _context.AppContent.Where(c => c.Type == (Domain.Enum.AppContentTypeEnum)request.type).FirstOrDefaultAsync();

        if (existingAppContent == null)
            throw new NotFoundException("Content type not found");

        if (!string.IsNullOrEmpty(request.htmlContent))
            existingAppContent.UpdateValue(request.htmlContent);

        _context.AppContent.Update(existingAppContent);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

