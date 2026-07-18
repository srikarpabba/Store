using Application.Abstractions.Messaging;

namespace Application.Banners.DeleteBanner;

public sealed record DeleteBannerCommand(Guid Id) : ICommand;
