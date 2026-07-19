using Application.Abstractions.Messaging;

namespace Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    IReadOnlyCollection<Guid> GenderIds,
    IReadOnlyCollection<Guid> SizeIds)
    : ICommand<Guid>;
