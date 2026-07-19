using Application.Abstractions.Messaging;

namespace Application.Categories.ReorderCategories;

/// <summary>
/// Persists the storefront display order of one gender's categories: each
/// category's sort order for that gender becomes its index in
/// <see cref="CategoryIds"/>. Other genders' orders are untouched.
/// </summary>
public sealed record ReorderCategoriesCommand(
    Guid GenderId,
    IReadOnlyList<Guid> CategoryIds) : ICommand;
