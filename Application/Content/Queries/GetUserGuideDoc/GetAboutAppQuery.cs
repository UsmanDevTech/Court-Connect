using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Content.Queries;

public sealed record GetAboutAppQuery : IRequest<AboutAppContract>;
internal sealed class GetAboutAppQueryHandler : IRequestHandler<GetAboutAppQuery, AboutAppContract>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IDateTime _datetime;
  
    public GetAboutAppQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser,
        IIdentityService identityService, IDateTime datetime)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _datetime = datetime;
    }

    public async Task<AboutAppContract> Handle(GetAboutAppQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var timeZoneId = _identityService.GetTimezone(userId);

        return new AboutAppContract
        {
            contactUs = await _context.AppContent.Where(x => x.Type == Domain.Enum.AppContentTypeEnum.ContactUs).Select(u => new GenericAppDocumentContract
            {
                id = u.Id,
                name = u.Name,
                iconUrl = u.Icon,
                value = u.Value,
            }).ToListAsync(cancellationToken),

            website = await _context.AppContent.Where(x => x.Type == Domain.Enum.AppContentTypeEnum.Webiste).Select(u => new GenericAppDocumentContract
            {
                id = u.Id,
                name = u.Name,
                iconUrl = u.Icon,
                value = u.Value,
            }).FirstOrDefaultAsync(cancellationToken),
            faq = await _context.AppContent.Where(x => x.Type == Domain.Enum.AppContentTypeEnum.Faq).Select(u => new GenericAppDocumentContract
            {
                id = u.Id,
                name = u.Name,
                iconUrl = u.Created.UtcToLocalTime(timeZoneId).ToString(_datetime.longDateFormat),
                value = u.Value,
            }).ToListAsync(cancellationToken),
        };
    }
}