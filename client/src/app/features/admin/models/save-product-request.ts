export interface SaveVariantRequest {
    /** update only — null for new variants, the variant id for existing
        ones; omitted entirely on create (the API rejects unknown fields) */
    id?: string | null;
    colorId: string;
    sizeId: string;
    price: number;
    quantityInStock: number;
    sku: string;
}

export interface SaveProductRequest {
    name: string;
    description: string;
    categoryId: string;
    brandId: string;
    genderIds: string[];
    variants: SaveVariantRequest[];
}
