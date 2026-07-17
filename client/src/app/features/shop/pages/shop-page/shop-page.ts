import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { combineLatest } from 'rxjs';
import { ShopService } from '../../services/shop.service';
import { ShopSection } from '../../models/enums/shop-section';
import { Product } from '../../models/product';
import { TitleCasePipe } from '@angular/common';
import { PricePipe } from '../../../../shared/pipes/price.pipe';

@Component({
  selector: 'app-shop-page',
  imports: [TitleCasePipe, PricePipe, RouterLink],
  templateUrl: './shop-page.html',
  styleUrl: './shop-page.css',
})
export class ShopPage {
  private route = inject(ActivatedRoute);

  private shopService = inject(ShopService);

  readonly section = signal('');

  readonly products = signal<Product[]>([]);

  ngOnInit(): void {

    combineLatest([
      this.route.paramMap,
      this.route.queryParamMap
    ]).subscribe(([params, queryParams]) => {

      const section =
        (params.get('section') as ShopSection) ?? ShopSection.New;

      const search = queryParams.get('search') ?? undefined;

      this.section.set(section);

      this.loadProducts(section, search);

    });
  }

  private async loadProducts(section: ShopSection, search?: string) {

    const response = await this.shopService
      .loadSection(section, search);

    response.subscribe(result => {

      this.products.set(result.items);

    });
  }
}
