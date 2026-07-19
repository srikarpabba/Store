using FluentValidation;

namespace Application.Subcategories.CreateSubcategory;

internal sealed class CreateSubcategoryCommandValidator : AbstractValidator<CreateSubcategoryCommand>
{
    public CreateSubcategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
