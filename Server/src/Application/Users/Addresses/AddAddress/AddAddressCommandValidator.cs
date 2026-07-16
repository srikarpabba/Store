using FluentValidation;

namespace Application.Users.Addresses.AddAddress;

internal sealed class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(c => c.Line1).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Line2).MaximumLength(200);
        RuleFor(c => c.City).NotEmpty().MaximumLength(100);
        RuleFor(c => c.State).NotEmpty().MaximumLength(100);
        RuleFor(c => c.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Country).NotEmpty().MaximumLength(100);
    }
}
