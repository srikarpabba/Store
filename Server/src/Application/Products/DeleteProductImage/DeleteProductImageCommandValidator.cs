using FluentValidation;

namespace Application.Products.DeleteProductImage;

internal sealed class DeleteProductImageCommandValidator
    : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.PhotoId).NotEmpty();
    }
}
