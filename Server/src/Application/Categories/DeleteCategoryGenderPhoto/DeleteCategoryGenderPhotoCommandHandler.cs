using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.DeleteCategoryGenderPhoto;

internal sealed class DeleteCategoryGenderPhotoCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<DeleteCategoryGenderPhotoCommand>
{
    public async Task<Result> Handle(DeleteCategoryGenderPhotoCommand command, CancellationToken cancellationToken)
    {
        CategoryGender? categoryGender = await context.CategoryGenders
            .FirstOrDefaultAsync(
                cg => cg.CategoryId == command.CategoryId && cg.GenderId == command.GenderId,
                cancellationToken);

        if (categoryGender is null)
        {
            return Result.Failure(CategoryErrors.GenderNotAssociated(command.CategoryId, command.GenderId));
        }

        string? photoFileName = categoryGender.PhotoFileName;

        if (photoFileName is null)
        {
            return Result.Success();
        }

        categoryGender.RemovePhoto();

        await context.SaveChangesAsync(cancellationToken);

        try
        {
            await fileStorage.DeleteAsync(photoFileName, cancellationToken);
        }
        catch (Exception)
        {
            // swallow: blob cleanup can be retried out of band
        }

        return Result.Success();
    }
}
