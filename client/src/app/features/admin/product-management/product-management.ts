import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

interface ProductManagementSection {
  icon: string;
  title: string;
  description: string;
  link: string;
}

@Component({
  selector: 'app-product-management',
  imports: [MatIconModule, RouterLink],
  templateUrl: './product-management.html',
  styleUrl: './product-management.css',
})
export class ProductManagement {

  readonly sections: ProductManagementSection[] = [
    {
      icon: 'inventory_2',
      title: 'Products',
      description: 'Create, edit and organise the catalog',
      link: '/admin/product-management/products'
    },
    {
      icon: 'category',
      title: 'Categories',
      description: 'Manage categories and their gender tags',
      link: '/admin/product-management/categories'
    },
    {
      icon: 'storefront',
      title: 'Brands',
      description: 'Manage brands and logos',
      link: '/admin/product-management/brands'
    }
  ];
}
