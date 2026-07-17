import { Component, effect, HostListener, input, model, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

/**
 * Right-hand slide-over panel with a backdrop. Content is projected, so it
 * can host anything (size guide, filters, cart preview…). Bind two-way:
 * `<app-drawer [(open)]="isOpen" title="Size guide">…</app-drawer>`
 */
@Component({
  selector: 'app-drawer',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './drawer.html',
  styleUrl: './drawer.css',
})
export class Drawer implements OnDestroy {

  readonly open = model(false);

  readonly title = input('');

  constructor() {
    // the page behind the drawer shouldn't scroll while it's open
    effect(() => {
      document.body.style.overflow = this.open() ? 'hidden' : '';
    });
  }

  ngOnDestroy(): void {
    document.body.style.overflow = '';
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.open()) {
      this.close();
    }
  }

  close(): void {
    this.open.set(false);
  }
}
