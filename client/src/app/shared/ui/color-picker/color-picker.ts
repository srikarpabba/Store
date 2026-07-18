import { Component, ElementRef, computed, effect, input, output, signal, untracked, viewChild } from '@angular/core';
import { NAMED_COLORS, NamedColor } from './named-colors';

interface Hsv {
  /** hue 0–360 */
  h: number;
  /** saturation 0–1 */
  s: number;
  /** value/brightness 0–1 */
  v: number;
}

/**
 * Inline HSV color picker — a saturation/value area plus a hue slider, driven
 * by pointer events so dragging updates live (unlike the native
 * `<input type="color">`, whose OS dialog is modal and blocks the page).
 * Two-way-ish: bind `[value]` (a `#RRGGBB` hex) in and listen to `(valueChange)`.
 */
@Component({
  selector: 'app-color-picker',
  templateUrl: './color-picker.html',
  styleUrl: './color-picker.css',
})
export class ColorPicker {

  private static readonly HEX = /^#[0-9A-Fa-f]{6}$/;

  readonly value = input<string>('#000000');

  readonly presets = input<readonly NamedColor[]>(NAMED_COLORS);

  readonly valueChange = output<string>();

  private readonly svArea = viewChild.required<ElementRef<HTMLElement>>('sv');

  private readonly hueArea = viewChild.required<ElementRef<HTMLElement>>('hue');

  readonly hsv = signal<Hsv>({ h: 0, s: 0, v: 0 });

  /** Pure hue at full saturation/value — the base color under the SV overlays. */
  readonly hueColor = computed(() => this.hsvToHex({ h: this.hsv().h, s: 1, v: 1 }));

  readonly svLeft = computed(() => this.hsv().s * 100);

  readonly svTop = computed(() => (1 - this.hsv().v) * 100);

  readonly hueLeft = computed(() => (this.hsv().h / 360) * 100);

  private readonly currentHex = computed(() => this.hsvToHex(this.hsv()));

  constructor() {
    // Sync from an externally-set hex (typed in the hex field / loaded). Only
    // when it genuinely differs from what we already show, so a value we just
    // emitted echoing back is a no-op. `untracked` keeps this effect firing on
    // `value()` alone, not on our own `hsv` writes (no feedback loop).
    effect(() => {
      const incoming = this.value();
      const current = untracked(() => this.currentHex());

      if (ColorPicker.HEX.test(incoming) && incoming.toLowerCase() !== current.toLowerCase()) {
        this.hsv.set(this.hexToHsv(incoming));
      }
    });
  }

  onSvPointerDown(event: PointerEvent): void {
    (event.target as HTMLElement).setPointerCapture(event.pointerId);
    this.updateSv(event);
  }

  onSvPointerMove(event: PointerEvent): void {
    if (event.buttons === 1) {
      this.updateSv(event);
    }
  }

  onHuePointerDown(event: PointerEvent): void {
    (event.target as HTMLElement).setPointerCapture(event.pointerId);
    this.updateHue(event);
  }

  onHuePointerMove(event: PointerEvent): void {
    if (event.buttons === 1) {
      this.updateHue(event);
    }
  }

  private updateSv(event: PointerEvent): void {
    const rect = this.svArea().nativeElement.getBoundingClientRect();
    const s = this.clamp01((event.clientX - rect.left) / rect.width);
    const v = 1 - this.clamp01((event.clientY - rect.top) / rect.height);
    this.commit({ h: this.hsv().h, s, v });
  }

  private updateHue(event: PointerEvent): void {
    const rect = this.hueArea().nativeElement.getBoundingClientRect();
    const h = this.clamp01((event.clientX - rect.left) / rect.width) * 360;
    this.commit({ h, s: this.hsv().s, v: this.hsv().v });
  }

  selectPreset(hex: string): void {
    this.hsv.set(this.hexToHsv(hex));
    this.valueChange.emit(hex.toUpperCase());
  }

  private commit(hsv: Hsv): void {
    this.hsv.set(hsv);
    this.valueChange.emit(this.hsvToHex(hsv));
  }

  private clamp01(n: number): number {
    return Math.min(1, Math.max(0, n));
  }

  private hsvToHex(hsv: Hsv): string {
    const c = hsv.v * hsv.s;
    const x = c * (1 - Math.abs(((hsv.h / 60) % 2) - 1));
    const m = hsv.v - c;

    let r = 0;
    let g = 0;
    let b = 0;

    if (hsv.h < 60) { r = c; g = x; }
    else if (hsv.h < 120) { r = x; g = c; }
    else if (hsv.h < 180) { g = c; b = x; }
    else if (hsv.h < 240) { g = x; b = c; }
    else if (hsv.h < 300) { r = x; b = c; }
    else { r = c; b = x; }

    const channel = (n: number) => Math.round((n + m) * 255).toString(16).padStart(2, '0');

    return `#${channel(r)}${channel(g)}${channel(b)}`.toUpperCase();
  }

  private hexToHsv(hex: string): Hsv {
    const r = parseInt(hex.slice(1, 3), 16) / 255;
    const g = parseInt(hex.slice(3, 5), 16) / 255;
    const b = parseInt(hex.slice(5, 7), 16) / 255;

    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    const d = max - min;

    let h = 0;

    if (d !== 0) {
      if (max === r) {
        h = 60 * (((g - b) / d) % 6);
      } else if (max === g) {
        h = 60 * ((b - r) / d + 2);
      } else {
        h = 60 * ((r - g) / d + 4);
      }
    }

    if (h < 0) {
      h += 360;
    }

    return {
      h,
      s: max === 0 ? 0 : d / max,
      v: max
    };
  }
}
