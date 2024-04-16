using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Accounts.Commands;

public sealed class CreateAccountCommand : IRequest<Result>
{
    public string? profilePic { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
    public string password { get; set; } = null!;
    public string? phoneNumber { get; set; }
    public string dateOfBirth { get; set; } = null!;
    public int gender { get; set; }
    public int level { get; set; }
    public int playingTennis { get; set; }
    public int playInMonth { get; set; }
    public int dtbPerformanceClass { get; set; }
    public string? clubName { get; set; }
    public double latitute { get; set; }
    public double longitute { get; set; }
    public string address { get; set; } = null!;
    public double radius { get; set; }
}
internal sealed class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result>
{
    private readonly IIdentityService _users;

    public CreateAccountCommandHandler(IIdentityService users)
    {
        _users = users;
    }

    public async Task<Result> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var result = await _users.CreateAccountAsync(request, cancellationToken);
        return result.Result;
    }
}
public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private readonly IIdentityService _userStore;
    public CreateAccountCommandValidator(IIdentityService users)
    {
        //Init Identity Repository
        _userStore = users;

        //Add Model Validation Using Fluent Validation
        RuleFor(u => u.email)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Email"))
            .EmailAddress().WithMessage(InvalidOperationErrorMessage.InvalidEmailFormat(""))
            .MustAsync(_userStore.BeUniqueEmailAsync).WithMessage(InvalidOperationErrorMessage.AlreadyExistsErrorMessage("User", "email"));

        RuleFor(u => u.phoneNumber)
          .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Phone Number"))
          .MustAsync(_userStore.BeUniquePhoneAsync).WithMessage(InvalidOperationErrorMessage.AlreadyExistsErrorMessage("User", "phone number"));

        RuleFor(u => u.password)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Password"))
            .MinimumLength(6).WithMessage(InvalidOperationErrorMessage.MinLengthErrorMessage("Password", 6));

        RuleFor(u => u.name)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Name"))
            .MaximumLength(60).WithMessage(InvalidOperationErrorMessage.MaxLengthErrorMessage("Name", 60));

        RuleFor(u => u.radius)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Radius"));

        RuleFor(u => u.latitute)
            .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Location"));

        RuleFor(u => u.longitute)
          .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Location"));

        RuleFor(u => u.address)
          .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Location"));

        RuleFor(u => u.dateOfBirth)
          .NotEmpty().WithMessage(InvalidOperationErrorMessage.IsRequiredErrorMessage("Date of birth"));
    }
}
