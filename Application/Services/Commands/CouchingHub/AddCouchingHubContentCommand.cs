using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;

namespace Application.Services.Commands;

public sealed class AddCouchingHubContentCommand : IRequest<Result>
{
    public int type { get; set; }
    public int contentType { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public double? price { get; set; }
    public string? thumbnail { get; set; }
    public string? url { get; set; }
    public long? timeLength { get; set; }
    public List<string> benifits { get; set; } = new();
    public int categoryId { get; set; }

}
internal sealed class AddCouchingHubContentCommandHandler : IRequestHandler<AddCouchingHubContentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    public AddCouchingHubContentCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }
    public async Task<Result> Handle(AddCouchingHubContentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var couchingHub = Domain.Entities.CouchingHub.Create(request.title, request.categoryId, (CouchingHubContentTypeEnum)request.type, request.price, _currentUser.UserId, _dateTime.NowUTC);
        _context.CouchingHubs.Add(couchingHub);

        if (request.benifits.Count() > 0)
        {
            couchingHub.AddCouchingHubBenifits(request.benifits, _dateTime.NowUTC);
        }
        couchingHub.AddCouchingHubContent((MediaTypeEnum)request.contentType, request.title, request.description,
            request.thumbnail, request.url, request.timeLength);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}