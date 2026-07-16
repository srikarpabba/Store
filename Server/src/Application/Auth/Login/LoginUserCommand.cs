using Application.Abstractions.Messaging;

namespace Application.Auth.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<AccessTokensResponse>;
