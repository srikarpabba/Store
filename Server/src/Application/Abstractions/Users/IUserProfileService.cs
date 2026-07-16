using Application.Users.Addresses;
using Application.Users.Addresses.AddAddress;
using Application.Users.Addresses.UpdateAddress;
using Application.Users.GetProfile;
using Application.Users.UpdateProfile;
using SharedKernel;

namespace Application.Abstractions.Users;

public interface IUserProfileService
{
    Task<Result<ProfileResponse>> GetProfileAsync(CancellationToken cancellationToken);
    Task<Result> UpdateProfileAsync(UpdateMyProfileCommand command, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<AddressResponse>>> GetAddressesAsync(CancellationToken cancellationToken);
    Task<Result<Guid>> AddAddressAsync(AddAddressCommand command, CancellationToken cancellationToken);
    Task<Result> UpdateAddressAsync(UpdateAddressCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken);
    Task<Result> SetDefaultAddressAsync(Guid addressId, CancellationToken cancellationToken);
}
