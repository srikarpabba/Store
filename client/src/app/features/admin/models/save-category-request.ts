export interface SaveCategoryRequest {
    name: string;
    description: string | null;
    genderIds: string[];
    sizeIds: string[];
}
