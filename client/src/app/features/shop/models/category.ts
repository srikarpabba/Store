export interface CategoryGenderInfo {
    genderId: string;
    genderName: string;
    photo: string | null;
}

export interface CategorySizeInfo {
    sizeId: string;
    sizeName: string;
}

export interface Category {

    id: string;

    name: string;

    description: string | null;

    genders: CategoryGenderInfo[];

    sizes: CategorySizeInfo[];
}
