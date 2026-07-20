/** datetime-local input value ("2026-12-31T14:30") -> ISO UTC string, or null when left blank */
export function toIsoOrNull(localValue: string): string | null {
    return localValue ? new Date(localValue).toISOString() : null;
}

/** ISO UTC string -> datetime-local input value in the browser's local time zone, or '' when unset */
export function toLocalInputValue(isoValue: string | null): string {
    if (!isoValue) {
        return '';
    }

    const date = new Date(isoValue);
    const offsetMs = date.getTimezoneOffset() * 60_000;
    return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
}

/** The current moment as a datetime-local input value — usable as a `min` so past dates can't be picked. */
export function nowLocalInputValue(): string {
    return toLocalInputValue(new Date().toISOString());
}
