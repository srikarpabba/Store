import { CategoryLookup } from './category-lookup';
import { Lookup } from './lookup';

export interface ProductFilters {

    brands: Lookup[];

    categories: CategoryLookup[];

    colors: Lookup[];

    sizes: Lookup[];

    genders: Lookup[];

    minPrice: number;

    maxPrice: number;
}