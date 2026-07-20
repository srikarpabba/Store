using FluentValidation;

namespace Application.Promotions.UpdatePromotion;

internal sealed class UpdatePromotionCommandValidator : AbstractValidator<UpdatePromotionCommand>
{
    public UpdatePromotionCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.DiscountPercentage).GreaterThan(0).LessThanOrEqualTo(100);

        RuleFor(x => x)
            .Must(x => x.ProductId.HasValue != x.BrandId.HasValue)
            .WithMessage("Exactly one of product or brand must be set.");

        RuleFor(x => x)
            .Must(x => !x.StartsAtUtc.HasValue || !x.EndsAtUtc.HasValue || x.EndsAtUtc > x.StartsAtUtc)
            .WithMessage("The end date must be after the start date.");
    }
}
