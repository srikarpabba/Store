using Application.Abstractions.Messaging;

namespace Application.Subcategories.UpdateSubcategory;

public sealed record UpdateSubcategoryCommand(Guid Id, string Name, Guid CategoryId) : ICommand;
