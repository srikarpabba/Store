import { Component, input } from '@angular/core';

/**
 * Decorative confetti-dot burst radiating from wherever it's placed —
 * absolutely fills its positioned parent, so drop it as a sibling of a
 * heart button inside a `position: relative` wrapper. Purely cosmetic:
 * plays whenever `active` is true and does nothing on its own to reset —
 * the caller flips `active` back to false once the animation has had time
 * to finish (see ProductCard/ProductDetails' justWishlisted signal).
 */
@Component({
  selector: 'app-heart-burst',
  imports: [],
  templateUrl: './heart-burst.html',
  styleUrl: './heart-burst.css',
})
export class HeartBurst {

  readonly active = input(false);

  protected readonly dots = Array.from({ length: 8 }, (_, i) => i);

  protected angleFor(index: number): string {
    return `${(360 / this.dots.length) * index}deg`;
  }
}
