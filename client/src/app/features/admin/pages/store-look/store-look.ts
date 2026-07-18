import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

interface StoreLookSection {
  icon: string;
  title: string;
  description: string;
  link: string;
}

@Component({
  selector: 'app-store-look',
  imports: [MatIconModule, RouterLink],
  templateUrl: './store-look.html',
  styleUrl: './store-look.css',
})
export class StoreLook {

  readonly sections: StoreLookSection[] = [
    {
      icon: 'view_carousel',
      title: 'Banners',
      description: 'Upload and order banner sliders shown on the Men, Women and Kids pages',
      link: '/admin/store-look/banners'
    }
  ];
}
