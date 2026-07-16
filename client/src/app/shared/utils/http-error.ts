import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extracts a user-friendly message from an API error.
 * The API returns RFC 7807 problem details, with validation
 * failures carried in an `errors` dictionary.
 */
export function extractHttpErrorMessage(error: HttpErrorResponse): string {

    // status 0 = the request never got a response; use the browser's
    // connectivity state to tell "you are offline" from "server is down"
    if (error.status === 0) {
        return navigator.onLine
            ? 'The server is unreachable right now. Please try again shortly.'
            : 'You appear to be offline. Please check your connection.';
    }

    if (error.status >= 500) {
        return 'The server ran into a problem. Please try again later.';
    }

    const problem = error.error;

    if (problem?.errors) {
        const firstError = Object.values(problem.errors).flat()[0];

        if (typeof firstError === 'string') {
            return firstError;
        }
    }

    return problem?.detail
        ?? problem?.title
        ?? 'Something went wrong. Please try again.';
}
