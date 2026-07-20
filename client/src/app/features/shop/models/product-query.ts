import { ProductSort } from "./enums/product-sort";

export interface ProductQuery {
    search?: string;

    brands?: string[];
    categories?: string[];
    subcategories?: string[];
    colors?: string[];
    sizes?: string[];
    genders?: string[];

    minPrice?: number;
    maxPrice?: number;

    /** Only products with a currently active promotion (their own or their brand's) */
    onSale?: boolean;

    sort?: ProductSort;

    pageIndex?: number;
    pageSize?: number;
}