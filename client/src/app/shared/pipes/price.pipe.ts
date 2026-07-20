import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats an amount in the store's currency (INR): whole amounts render
 * clean (₹999), fractional ones always show both paise digits (₹999.50,
 * never the "₹999.5" Intl.NumberFormat would give with a single shared
 * minimumFractionDigits — it pads to the minimum actually needed to
 * represent the value exactly, not a fixed width).
 * Single place to change locale/currency when the store expands.
 */
@Pipe({ name: 'price' })
export class PricePipe implements PipeTransform {

    private static readonly wholeFormatter = new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'INR',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });

    private static readonly fractionalFormatter = new Intl.NumberFormat('en-IN', {
        style: 'currency',
        currency: 'INR',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });

    transform(value: number | null | undefined): string {
        if (value == null) {
            return '';
        }

        // Rounds to the nearest paisa first so e.g. 999.001 (float noise)
        // still reads as a whole amount rather than spuriously "fractional"
        const rounded = Math.round(value * 100) / 100;

        return Number.isInteger(rounded)
            ? PricePipe.wholeFormatter.format(rounded)
            : PricePipe.fractionalFormatter.format(rounded);
    }
}
