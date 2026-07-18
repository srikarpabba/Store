import { inject, Service } from '@angular/core';
import { ProductService } from './product.service';
import { ProductSort } from '../models/enums/product-sort';
import { ShopSection } from '../models/enums/shop-section';
import { ProductQuery } from '../models/product-query';

@Service()
export class ShopService {
    private readonly productService = inject(ProductService);

    loadSection(section: ShopSection, search?: string, category?: string, pageIndex = 1) {

        const query: ProductQuery = { search, pageIndex, pageSize: 24 };

        if (category) {
            query.categories = [category];
        }

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

        return this.productService.getProductsGraphQl(query);
    }
}
