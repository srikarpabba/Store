using FluentValidation;

namespace Application.Products.GetProducts;

internal sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(q => q.PageIndex).GreaterThan(0);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);

        RuleFor(q => q.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(q => q.MinPrice.HasValue);

        RuleFor(q => q.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(q => q.MaxPrice.HasValue);

        RuleFor(q => q)
            .Must(q => !q.MinPrice.HasValue || !q.MaxPrice.HasValue || q.MinPrice <= q.MaxPrice)
            .WithMessage("MinPrice cannot be greater than MaxPrice");

        RuleFor(x => x.Search)
            .MaximumLength(100);
    }
}
