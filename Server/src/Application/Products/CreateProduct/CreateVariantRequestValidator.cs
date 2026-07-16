using FluentValidation;

namespace Application.Products.CreateProduct;

internal sealed class CreateVariantRequestValidator
    : AbstractValidator<CreateVariantRequest>
{
    public CreateVariantRequestValidator()
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
