import { Lookup } from './lookup';

export interface ProductFilters {

    brands: Lookup[];

    categories: Lookup[];

    colors: Lookup[];

    sizes: Lookup[];

    genders: Lookup[];

    minPrice: number;

    maxPrice: number;
}