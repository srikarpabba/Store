import { computed, Service, signal } from '@angular/core';

/**
 * Tracks in-flight API calls (incremented by the loading interceptor).
 * Mutations drive button spinners (isLoading); reads only feed the
 * header progress bar (isBusy) so background GETs like the search
 * typeahead never animate a submit button.
 */
@Service()
export class LoadingService {

    private readonly mutations = signal(0);
    private readonly reads = signal(0);

    /** A mutation (POST/PUT/PATCH/DELETE) is in flight — drives button spinners */
    readonly isLoading = computed(() => this.mutations() > 0);

    /** Any request is in flight — drives the header progress bar */
    readonly isBusy = computed(() => this.mutations() > 0 || this.reads() > 0);

    start(): void {
        this.mutations.update(count => count + 1);
    }

    stop(): void {
        this.mutations.update(count => Math.max(0, count - 1));
    }

    startRead(): void {
        this.reads.update(count => count + 1);
    }

    stopRead(): void {
        this.reads.update(count => Math.max(0, count - 1));
    }

    /** Escape hatch: clears a stuck loading state (e.g. after a hard failure). */
    reset(): void {
        this.mutations.set(0);
        this.reads.set(0);
    }
}
