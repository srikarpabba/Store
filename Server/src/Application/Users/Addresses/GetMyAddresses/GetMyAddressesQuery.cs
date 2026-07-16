using Application.Abstractions.Messaging;

namespace Application.Users.Addresses.GetMyAddresses;

public sealed record GetMyAddressesQuery : IQuery<IReadOnlyList<AddressResponse>>;
