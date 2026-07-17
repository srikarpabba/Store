using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.UploadCategoryGenderPhoto;

internal sealed class UploadCategoryGenderPhotoCommandHandler(
    IApplicationDbContext context,
    IFileStorage fileStorage)
    : ICommandHandler<UploadCategoryGenderPhotoCommand>
{
    public async Task<Result> Handle(UploadCategoryGenderPhotoCommand command, CancellationToken cancellationToken)
    {
        CategoryGender? categoryGender = await context.CategoryGenders
            .FirstOrDefaultAsync(
                cg => cg.CategoryId == command.CategoryId && cg.GenderId == command.GenderId,
                cancellationToken);

        if (categoryGender is null)
        {
            return Result.Failure(CategoryErrors.GenderNotAssociated(command.CategoryId, command.GenderId));
        }

        string? previousPhoto = categoryGender.PhotoFileName;

        string extension = Path.GetExtension(command.File.FileName);
        string objectKey = $"categories/{command.CategoryId}/{command.GenderId}/{Guid.NewGuid()}{extension}";

        await fileStorage.UploadAsync(command.File, objectKey, cancellationToken);

        categoryGender.SetPhoto(objectKey);

        await context.SaveChangesAsync(cancellationToken);

        if (previousPhoto is not null)
        {
            // best effort — the new photo is already saved either way
            try
            {
                await fileStorage.DeleteAsync(previousPhoto, cancellationToken);
            }
            catch (Exception)
            {
                // swallow: blob cleanup can be retried out of band
            }
        }

        return Result.Success();
    }
}
