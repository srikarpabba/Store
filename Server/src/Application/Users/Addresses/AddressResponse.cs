namespace Application.Users.Addresses;

public sealed record AddressResponse(
    Guid Id,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault);
