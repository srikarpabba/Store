using Application.Abstractions.Messaging;

namespace Application.Users.Addresses.UpdateAddress;

public sealed record UpdateAddressCommand(
    Guid AddressId,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country) : ICommand;
