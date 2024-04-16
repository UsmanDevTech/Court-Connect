using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Services.Commands;

public sealed record UpdateCategoryStatusCommand(int id, bool isDeleted) : IRequest<Result>;
internal sealed class UpdateCategoryStatusCommandHandler : IRequestHandler<UpdateCategoryStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
 
    public UpdateCategoryStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
      }

    public async Task<Result> Handle(UpdateCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = _context.CouchingHubCategories.Where(x => x.Id == request.id).FirstOrDefault();
        if (category == null)
            throw new NotFoundException("Category Not Found");

        if (request.isDeleted == true)
        {
            category.Deleted = false;
            _context.CouchingHubCategories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            category.Deleted = true;
            _context.CouchingHubCategories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}
