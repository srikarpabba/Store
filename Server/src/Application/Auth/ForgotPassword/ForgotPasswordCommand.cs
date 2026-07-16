using Application.Abstractions.Messaging;

namespace Application.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand;
