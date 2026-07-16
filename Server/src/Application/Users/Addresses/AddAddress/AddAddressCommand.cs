using Application.Abstractions.Messaging;

namespace Application.Users.Addresses.AddAddress;

public sealed record AddAddressCommand(
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country) : ICommand<Guid>;
