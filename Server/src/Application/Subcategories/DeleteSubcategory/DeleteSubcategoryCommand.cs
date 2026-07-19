using Application.Abstractions.Messaging;

namespace Application.Subcategories.DeleteSubcategory;

public sealed record DeleteSubcategoryCommand(Guid Id) : ICommand;
