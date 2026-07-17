using FluentValidation;

namespace Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description).MaximumLength(1000);

        RuleFor(x => x.GenderIds).NotEmpty()
            .WithMessage("Select at least one gender this category applies to.");
    }
}
