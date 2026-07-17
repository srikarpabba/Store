using Application.Abstractions.Messaging;

namespace Application.Auth.SetPassword;

public sealed record SetPasswordCommand(string NewPassword) : ICommand;
