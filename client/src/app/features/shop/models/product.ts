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

    /** Populated alongside category when the product has one — cards prefer
        showing this over the category name. */
    subcategory: Lookup | null;

    /** Percentage off, from whichever active promotion applies (its own or
        its brand's) — null when the product isn't currently on sale. */
    discountPercentage: number | null;
    saleEndsAtUtc: string | null;

    /** Only populated by the shop grid's GraphQL query (includeColors: true) — null for the admin table and search typeahead. */
    colors: ProductColorDetails[] | null;
}
