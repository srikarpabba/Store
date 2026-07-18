using Application.Common.Validation;
using FluentValidation;

namespace Application.Categories.UploadCategoryGenderPhoto;

internal sealed class UploadCategoryGenderPhotoCommandValidator : AbstractValidator<UploadCategoryGenderPhotoCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/avif"
    ];

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".avif"
    ];

    public UploadCategoryGenderPhotoCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();

        RuleFor(x => x.GenderId).NotEmpty();

        RuleFor(x => x.File).NotNull();

        RuleFor(x => x.File.FileName)
            .NotEmpty()
            .MaximumLength(255)
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid image extension.")
            .When(x => x.File is not null);

        RuleFor(x => x.File.ContentType)
            .Must(AllowedContentTypes.Contains)
            .WithMessage("Only JPEG, PNG, WebP and AVIF images are allowed.")
            .When(x => x.File is not null);

        RuleFor(x => x.File.Length)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage("The image must be 5 MB or smaller.")
            .When(x => x.File is not null);

        RuleFor(x => x.File)
            .MustAsync((file, cancellationToken) =>
                ImageSignatureValidator.MatchesDeclaredTypeAsync(file.Content, file.ContentType, cancellationToken))
            .WithMessage("The file's contents don't match a valid image.")
            .When(x => x.File is not null && AllowedContentTypes.Contains(x.File.ContentType));
    }
}
