using FluentValidation;

namespace Application.Brands.DeleteBrand;

internal sealed class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
    public DeleteBrandCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
