export enum StorefrontSectionType {
  Banner = 0,
  OfferStrip = 1,
  Product = 2,
  Category = 3,
  Collection = 4,
  Editorial = 5
}

export interface StorefrontBannerItem {
  id: string;
  title: string | null;
  link: string | null;
  photo: string | null;
  sortOrder: number;
}

export interface StorefrontCategoryItem {
  id: string;
  name: string;
  photo: string | null;
}

export interface StorefrontSection {
  key: string;
  title: string;
  type: StorefrontSectionType;
  displayOrder: number;
  items: unknown;
}

export interface StorefrontSections {
  sections: StorefrontSection[];
}
