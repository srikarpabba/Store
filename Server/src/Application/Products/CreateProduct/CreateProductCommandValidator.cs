using FluentValidation;

namespace Application.Products.CreateProduct;

internal sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(CreateVariantRequestValidator variantValidator)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.BrandId)
            .NotEmpty();

        RuleFor(x => x.GenderIds)
            .NotNull()
            .NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate genders are not allowed.");

        RuleFor(x => x.Variants)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Variants)
            .SetValidator(variantValidator);

        RuleFor(x => x.Variants)
            .Must(variants =>
                variants
                    .Select(v => new { v.ColorId, v.SizeId })
                    .Distinct()
                    .Count() == variants.Count)
            .WithMessage("Duplicate color/size combinations are not allowed.");

        RuleFor(x => x.Variants)
            .Must(variants =>
                variants
                    .Select(v => v.SKU)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() == variants.Count)
            .WithMessage("Duplicate SKUs are not allowed.");
    }
}
