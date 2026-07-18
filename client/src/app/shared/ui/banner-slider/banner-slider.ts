import { Component, DestroyRef, inject, input, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

export interface BannerSlide {
  id: string;
  title: string | null;
  link: string | null;
  photo: string | null;
}

/** Auto-advancing image carousel for storefront banners. */
@Component({
  selector: 'app-banner-slider',
  imports: [MatIconModule],
  templateUrl: './banner-slider.html',
  styleUrl: './banner-slider.css',
})
export class BannerSlider {

  private static readonly AUTOPLAY_MS = 5000;

  private readonly destroyRef = inject(DestroyRef);

  readonly banners = input.required<BannerSlide[]>();

  readonly activeIndex = signal(0);

  constructor() {
    const timer = setInterval(() => this.next(), BannerSlider.AUTOPLAY_MS);
    this.destroyRef.onDestroy(() => clearInterval(timer));
  }

  select(index: number): void {
    this.activeIndex.set(index);
  }

  next(): void {
    const count = this.banners().length;
    if (count > 0) {
      this.activeIndex.set((this.activeIndex() + 1) % count);
    }
  }

  previous(): void {
    const count = this.banners().length;
    if (count > 0) {
      this.activeIndex.set((this.activeIndex() - 1 + count) % count);
    }
  }
}
