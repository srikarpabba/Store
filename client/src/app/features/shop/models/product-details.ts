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
    brand: Lookup;
    rating: number;
    colors: ProductColorDetails[];
    genders: Lookup[];
    variants: ProductVariantDetails[];
}
