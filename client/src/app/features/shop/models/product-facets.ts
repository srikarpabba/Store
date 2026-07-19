export interface FacetCount {
    name: string;
    count: number;
}

/** Product counts per filter option under the current filter selection.
    Each facet's counts exclude that facet's own selection (server-side),
    so options within a facet stay additive. */
export interface ProductFacets {
    subcategories: FacetCount[];
    brands: FacetCount[];
    colors: FacetCount[];
    sizes: FacetCount[];
}
