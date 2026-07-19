/** A category option carrying the genders it's restricted to.
    An empty genderIds means the category is unisex — valid for any gender.
    Likewise an empty sizeIds means the category is not size-restricted. */
export interface CategoryLookup {

    id: string;

    name: string;

    genderIds: string[];

    sizeIds: string[];
}
