namespace Application.Auth.GoogleLogin;

public sealed record GoogleAuthResponse(string AccessToken, string RefreshToken, bool IsNewUser);
