using FluentValidation;

namespace Application.Users.UpdateProfile;

internal sealed class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.PhoneNumber)
            .Matches(@"^\+?[0-9\s-]{7,15}$")
            .When(c => !string.IsNullOrWhiteSpace(c.PhoneNumber))
            .WithMessage("Enter a valid phone number.");
    }
}
