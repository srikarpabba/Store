namespace Application.Products.GetFacets;

public sealed record FacetCount(string Name, int Count);

/// <summary>
/// Product counts per filter option under the current filter selection.
/// Each facet's counts are computed with every OTHER facet's selection
/// applied but its own excluded, so options within a facet stay additive
/// (picking "Nike" doesn't zero out the other brands).
/// </summary>
public sealed record ProductFacetsResponse(
    IReadOnlyList<FacetCount> Subcategories,
    IReadOnlyList<FacetCount> Brands,
    IReadOnlyList<FacetCount> Colors,
    IReadOnlyList<FacetCount> Sizes);
