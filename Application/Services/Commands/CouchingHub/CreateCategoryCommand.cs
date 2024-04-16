using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Commands;

public sealed class CreateCategoryCommand : IRequest<Result>
{
    public string name { get; set; }
    public string icon { get; set; }
}
internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityservice;
    private readonly IDateTime _datetime;
    public CreateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityservice, IDateTime datetime)
    {
        _context = context;
        _currentUser = currentUser;
        _identityservice = identityservice;
        _datetime = datetime;
    }

    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");


        CouchingHubCategory category = new(request.icon, request.name);
        _context.CouchingHubCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        //Return Value
        return Result.Success();


    }
}