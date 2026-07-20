import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

interface PromotionsSection {
  icon: string;
  title: string;
  description: string;
  /** Route of the section's page; absent while still a placeholder */
  link?: string;
}

@Component({
  selector: 'app-promotions-hub',
  imports: [MatIconModule, RouterLink],
  templateUrl: './promotions-hub.html',
  styleUrl: './promotions-hub.css',
})
export class PromotionsHub {

  readonly sections: PromotionsSection[] = [
    {
      icon: 'percent',
      title: 'Sales',
      description: 'Percentage-off discounts for a product or a whole brand, each with its own schedule — one at a time or many at once under a shared sale name',
      link: '/admin/promotions/sales'
    },
    {
      icon: 'confirmation_number',
      title: 'Discount Codes',
      description: 'Coupon codes shoppers redeem at checkout'
    },
    {
      icon: 'campaign',
      title: 'Campaigns',
      description: 'Grouped, themed promotions across multiple sales or codes'
    }
  ];
}
