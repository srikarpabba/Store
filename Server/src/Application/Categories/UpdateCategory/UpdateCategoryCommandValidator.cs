using FluentValidation;

namespace Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description).MaximumLength(1000);

        RuleFor(x => x.GenderIds).NotEmpty()
            .WithMessage("Select at least one gender this category applies to.");
    }
}
