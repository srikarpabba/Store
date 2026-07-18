using Application.Common.Validation;
using FluentValidation;

namespace Application.Products.UploadProductImages;

internal sealed class UploadProductImagesCommandValidator
    : AbstractValidator<UploadProductImagesCommand>
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

    public UploadProductImagesCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Files)
            .NotEmpty();

        RuleForEach(x => x.Files)
            .ChildRules(file =>
            {
                file.RuleFor(x => x.FileName)
                    .NotEmpty()
                    .MaximumLength(255)
                    .Must(name =>
                        AllowedExtensions.Contains(
                            Path.GetExtension(name),
                            StringComparer.OrdinalIgnoreCase))
                    .WithMessage("Invalid image extension.");

                file.RuleFor(x => x.ContentType)
                    .Must(AllowedContentTypes.Contains)
                    .WithMessage("Only JPEG, PNG, WebP and AVIF images are allowed.");

                file.RuleFor(x => x.Length)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(MaxFileSize)
                    .WithMessage("Each image must be 5 MB or smaller.");

                file.RuleFor(x => x.FileName)
                    .NotEmpty()
                    .MaximumLength(255);

                file.RuleFor(x => x)
                    .MustAsync((upload, cancellationToken) =>
                        ImageSignatureValidator.MatchesDeclaredTypeAsync(upload.Content, upload.ContentType, cancellationToken))
                    .WithMessage("A file's contents don't match a valid image.")
                    .When(upload => AllowedContentTypes.Contains(upload.ContentType));
            });

        RuleFor(x => x.Files)
            .Must(files => files.Count <= 10)
            .WithMessage("A maximum of 10 images can be uploaded at once.");

        RuleFor(x => x.Files)
            .Must(files => files.Select(f => f.FileName)
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .Count() == files.Count)
            .WithMessage("Duplicate file names are not allowed.");
    }
}
