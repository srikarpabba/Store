import { computed, effect, inject, Service, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { WishlistApi } from '../api/wishlist-api';
import { WishlistItem } from '../models/wishlist-item';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

/**
 * Holds the signed-in user's wishlist client-side so any heart icon can
 * reactively check membership without its own fetch. Reloads on login,
 * clears on logout, and re-syncs from the server after every add/remove
 * rather than patching local state optimistically — wishlists are small,
 * so the extra round trip is cheap and avoids local/server drift.
 */
@Service()
export class WishlistService {

    private readonly http = inject(HttpClient);
    private readonly authService = inject(AuthService);
    private readonly notificationService = inject(NotificationService);

    private readonly apiUrl = environment.apiUrl;

    private readonly _items = signal<WishlistItem[] | null>(null);

    readonly items = computed(() => this._items() ?? []);

    readonly isLoaded = computed(() => this._items() !== null);

    private readonly wishlistedIds = computed(() => new Set(this.items().map(i => i.productId)));

    constructor() {
        effect(() => {
            if (this.authService.isAuthenticated()) {
                this.refresh();
            } else {
                this._items.set([]);
            }
        });
    }

    isWishlisted(productId: string): boolean {
        return this.wishlistedIds().has(productId);
    }

    toggle(productId: string): void {
        if (this.isWishlisted(productId)) {
            this.remove(productId);
        } else {
            this.add(productId);
        }
    }

    add(productId: string): void {
        if (!this.requireSignedIn()) {
            return;
        }

        this.http.post<void>(`${this.apiUrl}${WishlistApi.item(productId)}`, {}).subscribe({
            next: () => this.refresh(),
            // failures are toasted by the error interceptor
            error: () => { }
        });
    }

    remove(productId: string): void {
        if (!this.requireSignedIn()) {
            return;
        }

        this.http.delete<void>(`${this.apiUrl}${WishlistApi.item(productId)}`).subscribe({
            next: () => this.refresh(),
            error: () => { }
        });
    }

    private refresh(): void {
        this.http.get<WishlistItem[]>(`${this.apiUrl}${WishlistApi.wishlist}`).subscribe({
            next: items => this._items.set(items),
            error: () => this._items.set([])
        });
    }

    private requireSignedIn(): boolean {
        if (this.authService.isAuthenticated()) {
            return true;
        }

        this.notificationService.info('Sign in to save items to your wishlist.');
        return false;
    }
}
