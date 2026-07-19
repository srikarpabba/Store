export const ProductApi = {
  products: '/products',
  filters: '/products/filters',
  facets: '/products/facets',
  details: (id: string) => `/products/${id}`
};