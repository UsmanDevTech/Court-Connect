
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enum;
using Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Content.Command;

public sealed record CreateAppContentCommand(int type, string name, string value, string? icon) : IRequest<Result>;
internal sealed class CreateAppContentCommandHandler : IRequestHandler<CreateAppContentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTimeService;
    public CreateAppContentCommandHandler(IApplicationDbContext context, IDateTime dateTimeService)
    {
        _context = context;
        _dateTimeService = dateTimeService;
    }
    public async Task<Result> Handle(CreateAppContentCommand request, CancellationToken cancellationToken)
    {
        var isExistingAppContent = _context.AppContent.Where(x => x.Name == request.name && x.Type == (AppContentTypeEnum)request.type).Any();
        if (isExistingAppContent)
            throw new CustomInvalidOperationException("Duplicate Record");

        _context.AppContent.Add(Domain.Entities.AppContent.Create(request.name, request.icon, request.value, (AppContentTypeEnum)request.type, _dateTimeService.NowUTC));

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class CreateAppContentCommandValidator : AbstractValidator<CreateAppContentCommand>
{
    public CreateAppContentCommandValidator()
    {
        RuleFor(u => u.name)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Name"));

        RuleFor(u => u.value)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Email"));
    }
}