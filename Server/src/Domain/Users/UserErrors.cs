using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error InvalidCredentials = Error.Problem(
        "Users.InvalidCredentials",
        "The email or password is incorrect.");

    public static readonly Error InvalidCurrentPassword = Error.Problem(
        "Users.InvalidCurrentPassword",
        "The current password is incorrect.");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");

    public static readonly Error InvalidRefreshToken = Error.Problem(
        "Users.InvalidRefreshToken",
        "The provided refresh token is invalid or has expired");

    public static readonly Error InvalidGoogleToken = Error.Problem(
        "Users.InvalidGoogleToken",
        "The provided Google token is invalid or has expired");

    public static readonly Error InvalidPasswordResetToken = Error.Problem(
        "Users.InvalidPasswordResetToken",
        "The password reset link is invalid or has expired");

    public static readonly Error InvalidEmailConfirmationToken = Error.Problem(
        "Users.InvalidEmailConfirmationToken",
        "The email confirmation link is invalid or has expired");

    public static readonly Error ConfirmationEmailRecentlySent = Error.Problem(
        "Users.ConfirmationEmailRecentlySent",
        "A confirmation email was sent recently. Please wait a minute before requesting another.");

    public static Error AddressNotFound(Guid addressId) => Error.NotFound(
        "Users.AddressNotFound",
        $"The address with the Id = '{addressId}' was not found");

    public static readonly Error RegistrationFailed = Error.Problem(
        "Users.RegistrationFailed",
        "Failed to register user.");

    public static readonly Error PasswordRequiresDigit = Error.Validation(
        "Users.PasswordRequiresDigit",
        "Password must contain at least one digit.");

    public static readonly Error PasswordRequiresLower = Error.Validation(
        "Users.PasswordRequiresLower",
        "Password must contain at least one lowercase letter.");

    public static readonly Error PasswordRequiresUpper = Error.Validation(
        "Users.PasswordRequiresUpper",
        "Password must contain at least one uppercase letter.");

    public static readonly Error PasswordRequiresNonAlphanumeric = Error.Validation(
        "Users.PasswordRequiresNonAlphanumeric",
        "Password must contain at least one special character.");

    public static readonly Error PasswordTooShort = Error.Validation(
        "Users.PasswordTooShort",
        "Password is too short.");
}
