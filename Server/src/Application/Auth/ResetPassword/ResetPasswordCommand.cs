using Application.Abstractions.Messaging;

namespace Application.Auth.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand;
