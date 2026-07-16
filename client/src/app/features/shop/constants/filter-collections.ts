export const FilterCollections = {
    Brands: 'brands',
    Categories: 'categories',
    Colors: 'colors',
    Sizes: 'sizes',
    Genders: 'genders'
} as const;

export type FilterCollection =
    typeof FilterCollections[keyof typeof FilterCollections];