using Application.Common.Constants;
using FluentValidation;

namespace Application.Banners.CreateBanner;

internal sealed class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.Storefront)
            .NotEmpty()
            .Must(Storefronts.IsValid)
            .WithMessage("Invalid storefront.");

        RuleFor(x => x.Title).MaximumLength(200);

        RuleFor(x => x.Link).MaximumLength(2048);
    }
}
