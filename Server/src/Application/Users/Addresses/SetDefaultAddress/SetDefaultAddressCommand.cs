using Application.Abstractions.Messaging;

namespace Application.Users.Addresses.SetDefaultAddress;

public sealed record SetDefaultAddressCommand(Guid AddressId) : ICommand;
