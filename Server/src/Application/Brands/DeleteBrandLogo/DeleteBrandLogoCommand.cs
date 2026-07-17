using Application.Abstractions.Messaging;

namespace Application.Brands.DeleteBrandLogo;

public sealed record DeleteBrandLogoCommand(Guid BrandId) : ICommand;
