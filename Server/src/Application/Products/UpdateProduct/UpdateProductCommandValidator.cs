using FluentValidation;

namespace Application.Products.UpdateProduct;

internal sealed class UpdateVariantRequestValidator
    : AbstractValidator<UpdateVariantRequest>
{
    public UpdateVariantRequestValidator()
    {
        RuleFor(x => x.ColorId)
            .NotEmpty();

        RuleFor(x => x.SizeId)
            .NotEmpty();

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-Za-z0-9\-_]+$")
            .WithMessage("SKU contains invalid characters.");
    }
}

internal sealed class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(UpdateVariantRequestValidator variantValidator)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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
