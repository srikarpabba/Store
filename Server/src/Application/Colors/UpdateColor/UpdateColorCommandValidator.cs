using FluentValidation;

namespace Application.Colors.UpdateColor;

internal sealed class UpdateColorCommandValidator : AbstractValidator<UpdateColorCommand>
{
    public UpdateColorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);

        RuleFor(x => x.HexCode)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Hex code must be in the form #RRGGBB.");
    }
}
