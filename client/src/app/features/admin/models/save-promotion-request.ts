export interface SavePromotionRequest {
    name: string;
    discountPercentage: number;
    startsAtUtc: string | null;
    endsAtUtc: string | null;
    isActive: boolean;
    productId: string | null;
    brandId: string | null;
}

export interface SavePromotionBatchItem {
    discountPercentage: number;
    startsAtUtc: string | null;
    endsAtUtc: string | null;
    isActive: boolean;
    productId: string | null;
    brandId: string | null;
}

/** Creates several promotions at once, all sharing one display name so they
    read as one sale event (e.g. "Diwali Sale") in the admin list. */
export interface SavePromotionBatchRequest {
    name: string;
    items: SavePromotionBatchItem[];
}
