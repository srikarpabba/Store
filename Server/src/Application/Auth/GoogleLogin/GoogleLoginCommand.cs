using Application.Abstractions.Messaging;

namespace Application.Auth.GoogleLogin;

public sealed record GoogleLoginCommand(string IdToken) : ICommand<GoogleAuthResponse>;
