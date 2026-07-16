using Application.Abstractions.Authentication;
using Application.Abstractions.Users;
using Application.Users.Addresses;
using Application.Users.Addresses.AddAddress;
using Application.Users.Addresses.UpdateAddress;
using Application.Users.GetProfile;
using Application.Users.UpdateProfile;
using Domain.Users;
using Infrastructure.Authentication;
using Infrastructure.Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Infrastructure.Users;

public class UserProfileService(
    UserManager<AppUser> userManager,
    ApplicationDbContext context,
    IUserContext userContext,
    IIdentityService identityService) : IUserProfileService
{
    public async Task<Result<ProfileResponse>> GetProfileAsync(CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user is null)
        {
            return Result.Failure<ProfileResponse>(UserErrors.Unauthorized());
        }

        return new ProfileResponse(
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.EmailConfirmed);
    }

    public async Task<Result> UpdateProfileAsync(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        user.FirstName = command.FirstName.Trim();
        user.LastName = command.LastName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(command.PhoneNumber)
            ? null
            : command.PhoneNumber.Trim();

        string newEmail = command.Email.Trim();
        bool emailChanged = !string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase);

        if (emailChanged)
        {
            AppUser? existing = await userManager.FindByEmailAsync(newEmail);

            if (existing is not null && existing.Id != user.Id)
            {
                return Result.Failure(UserErrors.EmailNotUnique);
            }

            // SetEmailAsync clears EmailConfirmed, so the new address
            // must be verified again
            IdentityResult setEmailResult = await userManager.SetEmailAsync(user, newEmail);

            if (!setEmailResult.Succeeded)
            {
                return Result.Failure(IdentityErrorMapper.Map(setEmailResult.Errors));
            }

            IdentityResult setUserNameResult = await userManager.SetUserNameAsync(user, newEmail);

            if (!setUserNameResult.Succeeded)
            {
                return Result.Failure(IdentityErrorMapper.Map(setUserNameResult.Errors));
            }
        }

        IdentityResult updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result.Failure(IdentityErrorMapper.Map(updateResult.Errors));
        }

        if (emailChanged)
        {
            // Best effort — profile update already succeeded, and the
            // dashboard offers a manual "verify email" resend
            await identityService.SendEmailConfirmationAsync(user.Id, cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<AddressResponse>>> GetAddressesAsync(CancellationToken cancellationToken)
    {
        List<AddressResponse> addresses = await context.Addresses
            .Where(a => a.UserId == userContext.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.CreatedOnUtc)
            .Select(a => new AddressResponse(
                a.Id,
                a.Line1,
                a.Line2,
                a.City,
                a.State,
                a.PostalCode,
                a.Country,
                a.IsDefault))
            .ToListAsync(cancellationToken);

        return addresses;
    }

    public async Task<Result<Guid>> AddAddressAsync(AddAddressCommand command, CancellationToken cancellationToken)
    {
        bool hasAddresses = await context.Addresses
            .AnyAsync(a => a.UserId == userContext.UserId, cancellationToken);

        var address = new Address
        {
            Line1 = command.Line1.Trim(),
            Line2 = string.IsNullOrWhiteSpace(command.Line2) ? null : command.Line2.Trim(),
            City = command.City.Trim(),
            State = command.State.Trim(),
            PostalCode = command.PostalCode.Trim(),
            Country = command.Country.Trim(),
            UserId = userContext.UserId,
            IsDefault = !hasAddresses
        };

        context.Addresses.Add(address);

        await context.SaveChangesAsync(cancellationToken);

        return address.Id;
    }

    public async Task<Result> UpdateAddressAsync(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        Address? address = await context.Addresses
            .SingleOrDefaultAsync(
                a => a.Id == command.AddressId && a.UserId == userContext.UserId,
                cancellationToken);

        if (address is null)
        {
            return Result.Failure(UserErrors.AddressNotFound(command.AddressId));
        }

        address.Line1 = command.Line1.Trim();
        address.Line2 = string.IsNullOrWhiteSpace(command.Line2) ? null : command.Line2.Trim();
        address.City = command.City.Trim();
        address.State = command.State.Trim();
        address.PostalCode = command.PostalCode.Trim();
        address.Country = command.Country.Trim();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken)
    {
        List<Address> addresses = await context.Addresses
            .Where(a => a.UserId == userContext.UserId)
            .ToListAsync(cancellationToken);

        Address? address = addresses.SingleOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            return Result.Failure(UserErrors.AddressNotFound(addressId));
        }

        context.Addresses.Remove(address);

        // Keep exactly one default when the default address is removed
        if (address.IsDefault)
        {
            Address? nextDefault = addresses
                .Where(a => a.Id != addressId)
                .OrderBy(a => a.CreatedOnUtc)
                .FirstOrDefault();

            nextDefault?.IsDefault = true;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetDefaultAddressAsync(Guid addressId, CancellationToken cancellationToken)
    {
        List<Address> addresses = await context.Addresses
            .Where(a => a.UserId == userContext.UserId)
            .ToListAsync(cancellationToken);

        Address? target = addresses.SingleOrDefault(a => a.Id == addressId);

        if (target is null)
        {
            return Result.Failure(UserErrors.AddressNotFound(addressId));
        }

        foreach (Address address in addresses)
        {
            address.IsDefault = address.Id == addressId;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
