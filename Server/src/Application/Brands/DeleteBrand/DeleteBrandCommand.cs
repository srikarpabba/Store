using Application.Abstractions.Messaging;

namespace Application.Brands.DeleteBrand;

public sealed record DeleteBrandCommand(Guid Id) : ICommand;
