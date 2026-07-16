import { ProductSort } from "./enums/product-sort";

export interface ProductQuery {
    search?: string;

    brands?: string[];
    categories?: string[];
    colors?: string[];
    sizes?: string[];
    genders?: string[];

    minPrice?: number;
    maxPrice?: number;

    sort?: ProductSort;

    pageIndex?: number;
    pageSize?: number;
}