import { Lookup } from './lookup';

export interface ProductPhoto {
    id: string;
    fileName: string;
    isMain: boolean;
}

export interface ProductColorDetails {
    productColorId: string;
    colorId: string;
    colorName: string;
    hexCode: string;
    photos: ProductPhoto[];
}

export interface ProductVariantDetails {
    id: string;
    productColorId: string;
    sizeId: string;
    sizeName: string;
    price: number;
    quantityInStock: number;
    sku: string;
}

export interface ProductDetails {
    id: string;
    name: string;
    description: string;
    category: Lookup;
    subcategory: Lookup | null;
    brand: Lookup;
    rating: number;
    /** Percentage off, from whichever active promotion applies (its own or
        its brand's) — null when the product isn't currently on sale. */
    discountPercentage: number | null;
    saleEndsAtUtc: string | null;
    colors: ProductColorDetails[];
    genders: Lookup[];
    variants: ProductVariantDetails[];
}
