export interface WishlistItem {
    productId: string;
    productName: string;
    image: string | null;
    startingPrice: number;
    discountPercentage: number | null;
    saleEndsAtUtc: string | null;
    createdOnUtc: string;
}
