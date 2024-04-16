using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Commands;

public sealed class PurchaseCouchingHubCommand : IRequest<Result>
{
    public int couchingContentId { get; set; }
    public string paymentIntent { get; set; }
    public double amountPaid { get; set; }
}
internal sealed class PurchaseCouchingHubCommandHandler : IRequestHandler<PurchaseCouchingHubCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    public IFormatProvider provider { get; private set; }
    public PurchaseCouchingHubCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(PurchaseCouchingHubCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var couchingContent = await _context.CouchingHubs.Include(x=>x.PurchasedClassess).Where(x => x.Id == request.couchingContentId).FirstOrDefaultAsync();
        if (couchingContent == null)
            throw new NotFoundException("Couching hub not found");

        var existingCouchingContent = couchingContent.PurchasedClassess.Where(x => x.CreatedBy == userId).Any();
        if (existingCouchingContent)
            throw new NotFoundException("Already purchased");

        couchingContent.AddCouchingPurchase(userId, couchingContent.Id,(double)request.amountPaid, 0.0, request.paymentIntent, _dateTime.NowUTC);
       
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
