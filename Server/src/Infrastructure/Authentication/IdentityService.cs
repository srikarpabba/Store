using Application.Abstractions.Authentication;
using Application.Abstractions.Email;
using Application.Auth;
using Application.Auth.ChangePassword;
using Application.Auth.ConfirmEmail;
using Application.Auth.ForgotPassword;
using Application.Auth.GoogleLogin;
using Application.Auth.Login;
using Application.Auth.Refresh;
using Application.Auth.Register;
using Application.Auth.ResetPassword;
using Application.Auth.SetPassword;
using Domain.Users;
using Google.Apis.Auth;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace Infrastructure.Authentication;

public class IdentityService(
    UserManager<AppUser> userManager,
    ITokenProvider tokenProvider,
    IDateTimeProvider dateTimeProvider,
    ApplicationDbContext context,
    IOptions<JwtOptions> options,
    IOptions<GoogleAuthOptions> googleOptions,
    IOptions<ClientOptions> clientOptions,
    IEmailService emailService,
    IUserContext userContext)
    : IIdentityService
{
    private const string GoogleLoginProvider = "Google";

    private readonly JwtOptions _jwtOptions = options.Value;
    private readonly GoogleAuthOptions _googleOptions = googleOptions.Value;
    private readonly ClientOptions _clientOptions = clientOptions.Value;

    public async Task<Result<Guid>> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = command.Email,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName
        };

        user.Raise(new UserRegisteredDomainEvent(user.Id));

        IdentityResult result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(IdentityErrorMapper.Map(result.Errors));
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, Roles.Customer);

        if (!addToRoleResult.Succeeded)
        {
            return Result.Failure<Guid>(IdentityErrorMapper.Map(addToRoleResult.Errors));
        }

        return user.Id;
    }

    public async Task<Result<AccessTokensResponse>> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidCredentials);
        }

        bool verified = await userManager.CheckPasswordAsync(user, command.Password);

        if (!verified)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidCredentials);
        }

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result<GoogleAuthResponse>> LoginWithGoogleAsync(GoogleLoginCommand command, CancellationToken cancellationToken)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                command.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_googleOptions.ClientId]
                });
        }
        catch (InvalidJwtException)
        {
            return Result.Failure<GoogleAuthResponse>(UserErrors.InvalidGoogleToken);
        }

        // Never trust an unverified email claim — matching by email below
        // could otherwise link a stranger's Google identity to an existing
        // (possibly privileged) account with that address
        if (!payload.EmailVerified)
        {
            return Result.Failure<GoogleAuthResponse>(UserErrors.InvalidGoogleToken);
        }

        AppUser? user = await userManager.FindByLoginAsync(GoogleLoginProvider, payload.Subject)
            ?? await userManager.FindByEmailAsync(payload.Email);

        bool isNewUser = user is null;

        if (user is null)
        {
            user = new AppUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                FirstName = payload.GivenName ?? string.Empty,
                LastName = payload.FamilyName ?? string.Empty,
                EmailConfirmed = payload.EmailVerified
            };

            user.Raise(new UserRegisteredDomainEvent(user.Id));

            IdentityResult createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return Result.Failure<GoogleAuthResponse>(IdentityErrorMapper.Map(createResult.Errors));
            }

            IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, Roles.Customer);

            if (!addToRoleResult.Succeeded)
            {
                return Result.Failure<GoogleAuthResponse>(IdentityErrorMapper.Map(addToRoleResult.Errors));
            }
        }

        IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);

        if (!logins.Any(l => l.LoginProvider == GoogleLoginProvider && l.ProviderKey == payload.Subject))
        {
            IdentityResult addLoginResult = await userManager.AddLoginAsync(
                user,
                new UserLoginInfo(GoogleLoginProvider, payload.Subject, GoogleLoginProvider));

            if (!addLoginResult.Succeeded)
            {
                return Result.Failure<GoogleAuthResponse>(IdentityErrorMapper.Map(addLoginResult.Errors));
            }
        }

        AccessTokensResponse tokens = await IssueTokensAsync(user, cancellationToken);

        return new GoogleAuthResponse(tokens.AccessToken, tokens.RefreshToken, isNewUser);
    }

    public async Task<Result<AccessTokensResponse>> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        string hashedRefreshToken = RefreshTokenHasher.Hash(command.RefreshToken);

        RefreshToken? refreshToken = await context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == hashedRefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidRefreshToken);
        }

        if (refreshToken.IsRevoked)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidRefreshToken);
        }

        DateTime now = dateTimeProvider.UtcNow;

        if (refreshToken.ExpiresOnUtc <= now)
        {
            refreshToken.Revoke(now);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidRefreshToken);
        }

        AppUser? user = await userManager.FindByIdAsync(refreshToken.UserId.ToString());

        if (user is null)
        {
            return Result.Failure<AccessTokensResponse>(UserErrors.InvalidRefreshToken);
        }

        refreshToken.Revoke(now);

        // IssueTokensAsync saves, persisting the revocation together
        // with the newly issued refresh token
        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        IdentityResult result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(IdentityErrorMapper.Map(result.Errors));
        }

        await context.RefreshTokens
                    .Where(x => x.UserId == user.Id && !x.IsRevoked)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(
                            x => x.RevokedOnUtc,
                            dateTimeProvider.UtcNow),
                        cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetPasswordAsync(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(userContext.UserId.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        // Only for accounts created via an external login (e.g. Google)
        // that never had a local password — existing passwords must go
        // through change password, which verifies the current one
        if (await userManager.HasPasswordAsync(user))
        {
            return Result.Failure(UserErrors.PasswordAlreadySet);
        }

        IdentityResult result = await userManager.AddPasswordAsync(user, command.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(IdentityErrorMapper.Map(result.Errors));
        }

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(command.Email);

        // Always report success so the endpoint doesn't reveal which emails exist
        if (user is null)
        {
            return Result.Success();
        }

        string token = await userManager.GeneratePasswordResetTokenAsync(user);

        string resetLink = $"{_clientOptions.BaseUrl}/account/reset-password" +
            $"?email={Uri.EscapeDataString(command.Email)}&token={Uri.EscapeDataString(token)}";

        await emailService.SendPasswordResetEmailAsync(command.Email, user.FirstName, resetLink, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SendEmailConfirmationAsync(Guid userId, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(userId));
        }

        // Google sign-ups arrive with a verified email — nothing to confirm
        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        // Throttle so the resend button can't be used to spam an inbox
        if (user.LastConfirmationEmailSent is { } lastSent
            && dateTimeProvider.UtcNow < lastSent.AddMinutes(1))
        {
            return Result.Failure(UserErrors.ConfirmationEmailRecentlySent);
        }

        string email = user.Email ?? throw new InvalidOperationException("User email is missing.");

        string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        string confirmationLink = $"{_clientOptions.BaseUrl}/account/confirm-email" +
            $"?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

        user.LastConfirmationEmailSent = dateTimeProvider.UtcNow;
        await userManager.UpdateAsync(user);

        await emailService.SendConfirmationEmailAsync(email, user.FirstName, confirmationLink, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        }

        // Link clicked twice — treat as already done rather than failing
        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        IdentityResult result = await userManager.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            return Result.Failure(UserErrors.InvalidPasswordResetToken);
        }

        IdentityResult result = await userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(IdentityErrorMapper.Map(result.Errors));
        }

        await context.RefreshTokens
            .Where(x => x.UserId == user.Id && !x.IsRevoked)
            .ExecuteUpdateAsync(
                s => s.SetProperty(
                    x => x.RevokedOnUtc,
                    dateTimeProvider.UtcNow),
                cancellationToken);

        return Result.Success();
    }

    private async Task<AccessTokensResponse> IssueTokensAsync(AppUser user, CancellationToken cancellationToken)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);

        string accessToken = tokenProvider.Create(user.Id, user.Email ?? throw new InvalidOperationException("User email is missing."), roles);
        string refreshToken = tokenProvider.GenerateRefreshToken();

        context.RefreshTokens.Add(new RefreshToken
        {
            Token = RefreshTokenHasher.Hash(refreshToken),
            UserId = user.Id,
            ExpiresOnUtc = dateTimeProvider.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationInDays)
        });

        await context.SaveChangesAsync(cancellationToken);

        return new AccessTokensResponse(accessToken, refreshToken);
    }
}
