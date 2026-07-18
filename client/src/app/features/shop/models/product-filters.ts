import { CategoryLookup } from './category-lookup';
import { ColorLookup } from './color-lookup';
import { Lookup } from './lookup';

export interface ProductFilters {

    brands: Lookup[];

    categories: CategoryLookup[];

    colors: ColorLookup[];

    sizes: Lookup[];

    genders: Lookup[];

    minPrice: number;

    maxPrice: number;
}