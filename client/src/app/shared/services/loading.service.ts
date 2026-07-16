import { computed, Service, signal } from '@angular/core';

/**
 * Tracks in-flight API calls (incremented by the loading interceptor).
 * Drives button spinners while a mutation is running.
 */
@Service()
export class LoadingService {

    private readonly activeRequests = signal(0);

    readonly isLoading = computed(() => this.activeRequests() > 0);

    start(): void {
        this.activeRequests.update(count => count + 1);
    }

    stop(): void {
        this.activeRequests.update(count => Math.max(0, count - 1));
    }

    /** Escape hatch: clears a stuck loading state (e.g. after a hard failure). */
    reset(): void {
        this.activeRequests.set(0);
    }
}
