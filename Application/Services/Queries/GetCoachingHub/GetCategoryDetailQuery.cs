using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Queries;
public sealed record GetCategoryDetailQuery(int categoryId) : IRequest<CouchingHubCategoryContract>;
internal sealed class GetCategoryDetailQueryHandler : IRequestHandler<GetCategoryDetailQuery, CouchingHubCategoryContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityservice;
    public GetCategoryDetailQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityservice = identityService;
    }

    public async Task<CouchingHubCategoryContract> Handle(GetCategoryDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        return await _context.CouchingHubCategories.Where(x => x.Id == request.categoryId).Select(u => new CouchingHubCategoryContract
        {
            id = u.Id,
            name = u.Name,
            icon = u.Icon,
            deleted = u.Deleted,
        }).FirstOrDefaultAsync(cancellationToken);
    }
}

