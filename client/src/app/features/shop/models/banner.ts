export interface Banner {

    id: string;

    storefront: string;

    title: string | null;

    link: string | null;

    photo: string | null;

    sortOrder: number;

    isActive: boolean;
}
