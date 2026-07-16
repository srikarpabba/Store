using Application.Abstractions.Messaging;

namespace Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AccessTokensResponse>;
