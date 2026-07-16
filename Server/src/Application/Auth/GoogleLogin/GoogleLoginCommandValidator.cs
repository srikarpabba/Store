using FluentValidation;

namespace Application.Auth.GoogleLogin;

internal sealed class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(c => c.IdToken).NotEmpty();
    }
}
