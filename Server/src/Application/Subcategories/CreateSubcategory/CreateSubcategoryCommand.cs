using Application.Abstractions.Messaging;

namespace Application.Subcategories.CreateSubcategory;

public sealed record CreateSubcategoryCommand(string Name, Guid CategoryId) : ICommand<Guid>;
