import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

interface AdminSection {
  icon: string;
  title: string;
  description: string;
  /** Route of the section's page; absent while still a placeholder */
  link?: string;
}

@Component({
  selector: 'app-admin-dashboard',
  imports: [MatIconModule, RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {

  /** Placeholders — each becomes its own admin page as features land */
  readonly sections: AdminSection[] = [
    {
      icon: 'inventory_2',
      title: 'Product Management',
      description: 'Products, categories and brands',
      link: '/admin/product-management'
    },
    {
      icon: 'auto_awesome_mosaic',
      title: 'Store Look',
      description: 'Banners and other storefront visual sections',
      link: '/admin/store-look'
    },
    {
      icon: 'receipt_long',
      title: 'Orders',
      description: 'Track, fulfil and refund customer orders'
    },
    {
      icon: 'group',
      title: 'Customers',
      description: 'View accounts, roles and activity'
    },
    {
      icon: 'sell',
      title: 'Promotions',
      description: 'Discount codes, campaigns and sales',
      link: '/admin/promotions'
    },
    {
      icon: 'insights',
      title: 'Analytics',
      description: 'Revenue, conversion and traffic reports'
    },
    {
      icon: 'settings',
      title: 'Store Settings',
      description: 'Shipping, taxes and storefront options'
    }
  ];
}
