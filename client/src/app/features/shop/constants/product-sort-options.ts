import { ProductSort } from "../models/enums/product-sort";

export const PRODUCT_SORT_OPTIONS = [
    {
        value: ProductSort.Newest,
        label: 'Newest'
    },
    {
        value: ProductSort.PriceLowToHigh,
        label: 'Price: Low to High'
    },
    {
        value: ProductSort.PriceHighToLow,
        label: 'Price: High to Low'
    },
    {
        value: ProductSort.Rating,
        label: 'Highest Rated'
    },
    {
        value: ProductSort.Name,
        label: 'Name (A–Z)'
    }
];