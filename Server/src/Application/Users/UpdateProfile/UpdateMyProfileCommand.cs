using Application.Abstractions.Messaging;

namespace Application.Users.UpdateProfile;

public sealed record UpdateMyProfileCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber) : ICommand;
