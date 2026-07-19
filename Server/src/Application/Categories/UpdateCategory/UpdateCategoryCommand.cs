using Application.Abstractions.Messaging;

namespace Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyCollection<Guid> GenderIds,
    IReadOnlyCollection<Guid> SizeIds)
    : ICommand;
