using FluentValidation;

namespace Application.Brands.DeleteBrandLogo;

internal sealed class DeleteBrandLogoCommandValidator : AbstractValidator<DeleteBrandLogoCommand>
{
    public DeleteBrandLogoCommandValidator()
    {
        RuleFor(x => x.BrandId).NotEmpty();
    }
}
