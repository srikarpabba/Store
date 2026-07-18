using Application.Common.Constants;
using FluentValidation;

namespace Application.Banners.UpdateBanner;

internal sealed class UpdateBannerCommandValidator : AbstractValidator<UpdateBannerCommand>
{
    public UpdateBannerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Storefront)
            .NotEmpty()
            .Must(Storefronts.IsValid)
            .WithMessage("Invalid storefront.");

        RuleFor(x => x.Title).MaximumLength(200);

        RuleFor(x => x.Link).MaximumLength(2048);
    }
}
