export interface CategoryGenderInfo {
    genderId: string;
    genderName: string;
    photo: string | null;
}

export interface Category {

    id: string;

    name: string;

    description: string | null;

    genders: CategoryGenderInfo[];
}
