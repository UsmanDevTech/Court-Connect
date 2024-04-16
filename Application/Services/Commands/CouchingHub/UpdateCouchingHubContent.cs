using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enum;
using MediatR;

namespace Application.Services.Commands;

public sealed class UpdateCouchingHubContent : IRequest<Result>
{
    public int id { get; set; }
    public int contentType { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public double? price { get; set; }
    public string? thumbnail { get; set; }
    public string? url { get; set; }
    public List<string> benifits { get; set; } = new();
}
internal sealed class UpdateCouchingHubContentHandler : IRequestHandler<UpdateCouchingHubContent, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _datetimeService;
    public UpdateCouchingHubContentHandler(IApplicationDbContext context, IDateTime datetimeService)
    {
        _context = context;
        _datetimeService = datetimeService;
    }
    public async Task<Result> Handle(UpdateCouchingHubContent request, CancellationToken cancellationToken)
    {
        var existingCouchingHub = await _context.CouchingHubs.FindAsync(request.id);

        if (existingCouchingHub == null)
            throw new NotFoundException("Couching hub not found");

        if (!string.IsNullOrEmpty(request.title))
            existingCouchingHub.UpdateTitle(request.title);
        if (request.price.HasValue)
            existingCouchingHub.UpdatePrice((double)request.price);


        var existingCouchingDetail = _context.CouchingHubDetails.Where(x => x.CouchingHubId == request.id).FirstOrDefault();

        if (!string.IsNullOrEmpty(request.description))
            existingCouchingDetail.UpdateDescription(request.description);
        if (!string.IsNullOrEmpty(request.thumbnail))
            existingCouchingDetail.UpdateThumbnail(request.thumbnail);
        if (!string.IsNullOrEmpty(request.url))
            existingCouchingDetail.UpdateUrl(request.url);

        existingCouchingDetail.UpdateType((MediaTypeEnum)request.contentType);

        if (request.benifits.Count() > 0)
        {
            existingCouchingHub.AddCouchingHubBenifits(request.benifits, _datetimeService.NowUTC);
        }
       
        _context.CouchingHubs.Update(existingCouchingHub);
        _context.CouchingHubDetails.Update(existingCouchingDetail);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
}
