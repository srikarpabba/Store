using FluentValidation;

namespace Application.Products.SetMainProductImage;

internal sealed class SetMainProductImageCommandValidator
    : AbstractValidator<SetMainProductImageCommand>
{
    public SetMainProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.PhotoId).NotEmpty();
    }
}
