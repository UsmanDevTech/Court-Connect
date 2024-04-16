using Application.Common.Interfaces;
using Domain.Contracts;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Content.Queries;
public sealed record GetUserGuideDocQuery(AppContentTypeEnum type) : IRequest<GenericAppDocumentContract>;
internal sealed class GetUserGuideDocQueryHandler : IRequestHandler<GetUserGuideDocQuery, GenericAppDocumentContract>
{
    private readonly IApplicationDbContext _context;
    public GetUserGuideDocQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GenericAppDocumentContract> Handle(GetUserGuideDocQuery request, CancellationToken cancellationToken)
    {
        return await _context.AppContent.Where(u => u.Type == request.type).Select(u => new GenericAppDocumentContract
        {
            id = u.Id,
            name = u.Name,
            value = u.Value,
            iconUrl = u.Icon,
        }).FirstOrDefaultAsync(cancellationToken) ?? new GenericAppDocumentContract();
    }
}
