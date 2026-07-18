using FluentValidation;

namespace Application.Colors.CreateColor;

internal sealed class CreateColorCommandValidator : AbstractValidator<CreateColorCommand>
{
    public CreateColorCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);

        RuleFor(x => x.HexCode)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Hex code must be in the form #RRGGBB.");
    }
}
