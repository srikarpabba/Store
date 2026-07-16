using Application.Abstractions.Messaging;

namespace Application.Auth.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : ICommand;
