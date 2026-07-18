using FluentValidation;

namespace Application.Banners.DeleteBanner;

internal sealed class DeleteBannerCommandValidator : AbstractValidator<DeleteBannerCommand>
{
    public DeleteBannerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
