import { Lookup } from './lookup';
import { ProductColorDetails } from './product-details';

export interface Product {

    id: string;

    name: string;

    startingPrice: number;

    rating: number;

    image: string | null;

    /** Only populated by the shop grid's GraphQL query (includeColors: true) — null for the admin table and search typeahead. */
    category: Lookup | null;

    /** Only populated by the shop grid's GraphQL query (includeColors: true) — null for the admin table and search typeahead. */
    colors: ProductColorDetails[] | null;
}
