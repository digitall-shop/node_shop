using Domain.DTOs.Panel;
using FluentValidation;

namespace Application.Validators;

public class PanelValidator : AbstractValidator<PanelCreateDto>
{
    public PanelValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("must have 100 characters");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required")
            .MaximumLength(250)
            .WithMessage("URL must have 250 characters")
            .Must(uri =>
                Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid URL");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required")
            .MaximumLength(150)
            .WithMessage("UserNme must have 150 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters")
            .MaximumLength(250)
            .WithMessage("Password must have 250 characters");
    }
}