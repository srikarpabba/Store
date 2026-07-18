export interface SaveBannerRequest {
    storefront: string;
    title: string | null;
    link: string | null;
    sortOrder: number;
    isActive: boolean;
}
