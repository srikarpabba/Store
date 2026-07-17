namespace Application.Users.GetProfile;

public sealed record ProfileResponse(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool HasPassword);
