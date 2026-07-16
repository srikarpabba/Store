import { inject, Service } from '@angular/core';
import { ProductService } from './product.service';
import { ProductSort } from '../models/enums/product-sort';
import { ShopSection } from '../models/enums/shop-section';
import { ProductQuery } from '../models/product-query';

@Service()
export class ShopService {
    private readonly productService = inject(ProductService);

    loadSection(section: ShopSection, search?: string) {

        const query: ProductQuery = { search };

        switch (section) {

            case ShopSection.Men:
                query.genders = ['Male'];
                break;

            case ShopSection.Women:
                query.genders = ['Female'];
                break;

            case ShopSection.New:
                query.sort = ProductSort.Newest;
                break;

            case ShopSection.Sale:
                // TODO
                break;
        }

        return this.productService.getProducts(query);
    }
}
