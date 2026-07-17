using FluentValidation;

namespace Application.Categories.DeleteCategoryGenderPhoto;

internal sealed class DeleteCategoryGenderPhotoCommandValidator : AbstractValidator<DeleteCategoryGenderPhotoCommand>
{
    public DeleteCategoryGenderPhotoCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();

        RuleFor(x => x.GenderId).NotEmpty();
    }
}
