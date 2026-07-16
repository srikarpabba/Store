using Domain.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Infrastructure.Authentication;

internal static class IdentityErrorMapper
{
    private static readonly Dictionary<string, Error> IdentityErrors = new()
    {
        [nameof(IdentityErrorDescriber.DuplicateEmail)] = UserErrors.EmailNotUnique,
        [nameof(IdentityErrorDescriber.PasswordRequiresDigit)] = UserErrors.PasswordRequiresDigit,
        [nameof(IdentityErrorDescriber.PasswordRequiresLower)] = UserErrors.PasswordRequiresLower,
        [nameof(IdentityErrorDescriber.PasswordRequiresUpper)] = UserErrors.PasswordRequiresUpper,
        [nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)] = UserErrors.PasswordRequiresNonAlphanumeric,
        [nameof(IdentityErrorDescriber.PasswordTooShort)] = UserErrors.PasswordTooShort,
        [nameof(IdentityErrorDescriber.PasswordMismatch)] = UserErrors.InvalidCurrentPassword
    };

    public static ValidationError Map(IEnumerable<IdentityError> identityErrors)
    {
        Error[] errors = identityErrors
            .Select(error =>
                IdentityErrors.TryGetValue(error.Code, out Error? mappedError)
                    ? mappedError
                    : Error.Problem(error.Code, error.Description))
            .ToArray();

        return new ValidationError(errors);
    }
}
