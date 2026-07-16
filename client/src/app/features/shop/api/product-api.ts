export const ProductApi = {
  products: '/products',
  filters: '/products/filters',
  details: (id: string) => `/products/${id}`
};