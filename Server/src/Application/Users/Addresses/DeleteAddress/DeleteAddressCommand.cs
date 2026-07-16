using Application.Abstractions.Messaging;

namespace Application.Users.Addresses.DeleteAddress;

public sealed record DeleteAddressCommand(Guid AddressId) : ICommand;
