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

public sealed class UpdateCategoryCommand : IRequest<Result>
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? icon { get; set; }
}
internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityservice;
    public UpdateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IIdentityService identityservice)
    {
        _context = context;
        _currentUser = currentUser;
        _identityservice = identityservice;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var category = _context.CouchingHubCategories.Where(x => x.Id == request.id).FirstOrDefault();
        if (category == null)
            throw new NotFoundException("Category not found");

        var iconvalue = "";
        var name = "";

        if (!string.IsNullOrEmpty(request.icon))
        {
            iconvalue = request.icon;
        }
        else
        {
            iconvalue = category.Icon;
        }

        if (!string.IsNullOrEmpty(request.name))
        {
            name = request.name;
        }
        else
        {
            name = category.Name;
        }

        category.SetNameandIcon(iconvalue, name);

        _context.CouchingHubCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);

        //Return Value
        return Result.Success();


    }
}
