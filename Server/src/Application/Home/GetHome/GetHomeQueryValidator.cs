using Application.Common.Constants;
using FluentValidation;

namespace Application.Home.GetHome;

internal sealed class GetHomeQueryValidator : AbstractValidator<GetHomeQuery>
{
    public GetHomeQueryValidator()
    {
        RuleFor(x => x.Storefront)
            .NotEmpty()
            .Must(storefront =>
            {
                storefront = storefront.Trim().ToUpperInvariant();

                return storefront is
                    Storefronts.Men or
                    Storefronts.Women or
                    Storefronts.Kids;
            })
            .WithMessage("Invalid storefront.");
    }
}
