using FluentValidation;

namespace Application.Promotions.CreatePromotionBatch;

internal sealed class CreatePromotionBatchCommandValidator : AbstractValidator<CreatePromotionBatchCommand>
{
    public CreatePromotionBatchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Items).NotEmpty().WithMessage("Add at least one product or brand.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.DiscountPercentage).GreaterThan(0).LessThanOrEqualTo(100);

            item.RuleFor(i => i)
                .Must(i => i.ProductId.HasValue != i.BrandId.HasValue)
                .WithMessage("Exactly one of product or brand must be set.");

            item.RuleFor(i => i)
                .Must(i => !i.StartsAtUtc.HasValue || !i.EndsAtUtc.HasValue || i.EndsAtUtc > i.StartsAtUtc)
                .WithMessage("The end date must be after the start date.");
        });
    }
}
