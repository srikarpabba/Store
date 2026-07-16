import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

interface AdminSection {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-admin-dashboard',
  imports: [MatIconModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard {

  /** Placeholders — each becomes its own admin page as features land */
  readonly sections: AdminSection[] = [
    {
      icon: 'inventory_2',
      title: 'Products',
      description: 'Create, edit and organise the catalog'
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
      description: 'Discount codes, sales and campaigns'
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
