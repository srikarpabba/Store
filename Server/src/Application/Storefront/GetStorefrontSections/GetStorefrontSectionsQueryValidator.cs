using Application.Common.Constants;
using FluentValidation;

namespace Application.Storefront.GetStorefrontSections;

internal sealed class GetStorefrontSectionsQueryValidator : AbstractValidator<GetStorefrontSectionsQuery>
{
    public GetStorefrontSectionsQueryValidator()
    {
        RuleFor(x => x.Storefront)
            .NotEmpty()
            .Must(Storefronts.IsValid)
            .WithMessage("Invalid storefront.");
    }
}
