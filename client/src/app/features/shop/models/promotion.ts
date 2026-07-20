/** A percentage-off sale scoped to exactly one product or one brand. */
export interface Promotion {

    id: string;

    name: string;

    discountPercentage: number;

    startsAtUtc: string | null;

    endsAtUtc: string | null;

    isActive: boolean;

    productId: string | null;

    productName: string | null;

    brandId: string | null;

    brandName: string | null;
}
