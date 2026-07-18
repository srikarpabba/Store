export interface NamedColor {
    name: string;
    hex: string;
}

/** Curated palette — doubles as the picker's preset swatches and the source
 *  for nearest-name lookups when auto-filling a color's name. */
export const NAMED_COLORS: readonly NamedColor[] = [
    { name: 'Black', hex: '#000000' },
    { name: 'White', hex: '#FFFFFF' },
    { name: 'Gray', hex: '#808080' },
    { name: 'Silver', hex: '#C0C0C0' },
    { name: 'Red', hex: '#FF0000' },
    { name: 'Maroon', hex: '#800000' },
    { name: 'Crimson', hex: '#DC143C' },
    { name: 'Salmon', hex: '#FA8072' },
    { name: 'Coral', hex: '#FF7F50' },
    { name: 'Orange', hex: '#FFA500' },
    { name: 'Gold', hex: '#FFD700' },
    { name: 'Yellow', hex: '#FFFF00' },
    { name: 'Olive', hex: '#808000' },
    { name: 'Khaki', hex: '#F0E68C' },
    { name: 'Beige', hex: '#F5F5DC' },
    { name: 'Lime', hex: '#00FF00' },
    { name: 'Green', hex: '#008000' },
    { name: 'Teal', hex: '#008080' },
    { name: 'Turquoise', hex: '#40E0D0' },
    { name: 'Cyan', hex: '#00FFFF' },
    { name: 'Sky Blue', hex: '#87CEEB' },
    { name: 'Blue', hex: '#0000FF' },
    { name: 'Royal Blue', hex: '#4169E1' },
    { name: 'Navy', hex: '#000080' },
    { name: 'Indigo', hex: '#4B0082' },
    { name: 'Purple', hex: '#800080' },
    { name: 'Violet', hex: '#EE82EE' },
    { name: 'Magenta', hex: '#FF00FF' },
    { name: 'Pink', hex: '#FFC0CB' },
    { name: 'Hot Pink', hex: '#FF69B4' },
    { name: 'Brown', hex: '#A52A2A' },
    { name: 'Chocolate', hex: '#D2691E' },
    { name: 'Tan', hex: '#D2B48C' },
    { name: 'Lavender', hex: '#E6E6FA' }
];

/** Name of the palette color closest to the given hex (Euclidean RGB distance). */
export function nearestColorName(hex: string): string {
    const target = hexToRgb(hex);

    if (target === null) {
        return '';
    }

    let bestName = NAMED_COLORS[0].name;
    let bestDistance = Number.POSITIVE_INFINITY;

    for (const color of NAMED_COLORS) {
        const rgb = hexToRgb(color.hex)!;
        const distance =
            (rgb.r - target.r) ** 2 +
            (rgb.g - target.g) ** 2 +
            (rgb.b - target.b) ** 2;

        if (distance < bestDistance) {
            bestDistance = distance;
            bestName = color.name;
        }
    }

    return bestName;
}

function hexToRgb(hex: string): { r: number; g: number; b: number } | null {
    if (!/^#[0-9A-Fa-f]{6}$/.test(hex)) {
        return null;
    }

    return {
        r: parseInt(hex.slice(1, 3), 16),
        g: parseInt(hex.slice(3, 5), 16),
        b: parseInt(hex.slice(5, 7), 16)
    };
}
