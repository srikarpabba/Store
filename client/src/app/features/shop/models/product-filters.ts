import { CategoryLookup } from './category-lookup';
import { ColorLookup } from './color-lookup';
import { Lookup } from './lookup';
import { SubcategoryLookup } from './subcategory-lookup';

export interface ProductFilters {

    brands: Lookup[];

    categories: CategoryLookup[];

    subcategories: SubcategoryLookup[];

    colors: ColorLookup[];

    sizes: Lookup[];

    genders: Lookup[];

    minPrice: number;

    maxPrice: number;
}