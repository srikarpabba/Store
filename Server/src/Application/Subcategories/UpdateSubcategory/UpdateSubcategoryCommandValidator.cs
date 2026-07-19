using FluentValidation;

namespace Application.Subcategories.UpdateSubcategory;

internal sealed class UpdateSubcategoryCommandValidator : AbstractValidator<UpdateSubcategoryCommand>
{
    public UpdateSubcategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
