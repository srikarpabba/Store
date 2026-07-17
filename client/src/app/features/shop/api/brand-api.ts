export const BrandApi = {
  brands: '/brands',
  details: (id: string) => `/brands/${id}`,
  logo: (id: string) => `/brands/${id}/logo`
};
