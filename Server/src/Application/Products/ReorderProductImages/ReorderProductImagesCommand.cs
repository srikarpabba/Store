using Application.Abstractions.Messaging;

namespace Application.Products.ReorderProductImages;

/// <summary>
/// Persists the display order of one color's photos: each photo's sort
/// order becomes its index in <see cref="PhotoIds"/>. The main photo still
/// always displays first regardless of where it lands in the list.
/// </summary>
public sealed record ReorderProductImagesCommand(
    Guid ProductId,
    Guid ProductColorId,
    IReadOnlyList<Guid> PhotoIds) : ICommand;
