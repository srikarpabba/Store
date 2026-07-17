import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats an amount in the store's currency (INR), e.g. 1499.5 → "₹1,499.50".
 * Single place to change locale/currency when the store expands.
 */
@Pipe({ name: 'price' })
export class PricePipe implements PipeTransform {

    private static readonly formatter = new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'INR',
        // whole amounts render clean (₹999), paise only when present (₹999.50)
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    });

    transform(value: number | null | undefined): string {
        return value == null ? '' : PricePipe.formatter.format(value);
    }
}
