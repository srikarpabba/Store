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
using SharedKernel;

namespace Application.Abstractions.Authentication;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    Task<Result<AccessTokensResponse>> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken);
    Task<Result<GoogleAuthResponse>> LoginWithGoogleAsync(GoogleLoginCommand command, CancellationToken cancellationToken);
    Task<Result<AccessTokensResponse>> RefreshTokenAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
    Task<Result> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken);
    Task<Result> SetPasswordAsync(SetPasswordCommand command, CancellationToken cancellationToken);
    Task<Result> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken);
    Task<Result> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
    Task<Result> SendEmailConfirmationAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken);
}
