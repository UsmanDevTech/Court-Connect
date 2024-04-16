using Application.Common.Interfaces;
using MediatR;

namespace Application.Accounts.Queries;


public record CheckIdentityValueUniquenessQuery(int type,  string value) : IRequest<bool>;
public class CheckIdentityValueUniquenessQueryHandler : IRequestHandler<CheckIdentityValueUniquenessQuery, bool>
{
    private readonly IIdentityService _identityService;

    public CheckIdentityValueUniquenessQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(CheckIdentityValueUniquenessQuery request, CancellationToken cancellationToken)
    {
        if (request.type == 0)
            return await _identityService.BeUniqueEmailAsync(request.value, cancellationToken);
        else if (request.type == 1)
            return await _identityService.BeUniquePhoneAsync(request.value, cancellationToken);
         return false;
    }
}
