/** Applies a percentage-off discount to a price, rounded to the nearest
    paisa (2 decimals) — kept at full currency precision rather than a whole
    rupee, since this value is the source of truth for what the store
    actually charges once checkout exists, not just a display label.
    PricePipe is responsible for formatting it cleanly either way. */
export function applyDiscount(price: number, discountPercentage: number): number {
    return Math.round(price * (1 - discountPercentage / 100) * 100) / 100;
}
