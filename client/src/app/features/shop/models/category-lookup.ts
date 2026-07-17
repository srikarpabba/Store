/** A category option carrying the genders it's restricted to.
    An empty genderIds means the category is unisex — valid for any gender. */
export interface CategoryLookup {

    id: string;

    name: string;

    genderIds: string[];
}
