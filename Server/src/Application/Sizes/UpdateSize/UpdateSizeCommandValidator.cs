using FluentValidation;

namespace Application.Sizes.UpdateSize;

internal sealed class UpdateSizeCommandValidator : AbstractValidator<UpdateSizeCommand>
{
    public UpdateSizeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(10);
    }
}
