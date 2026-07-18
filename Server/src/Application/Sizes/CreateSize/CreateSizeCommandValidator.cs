using FluentValidation;

namespace Application.Sizes.CreateSize;

internal sealed class CreateSizeCommandValidator : AbstractValidator<CreateSizeCommand>
{
    public CreateSizeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(10);
    }
}
