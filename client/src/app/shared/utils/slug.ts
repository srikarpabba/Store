/** URL-friendly slug for a display name: "T-Shirt" -> "t-shirt".
    Deterministic, so links and lookups both derive it from the name. */
export function slugify(name: string): string {
    return name
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/^-+|-+$/g, '');
}
