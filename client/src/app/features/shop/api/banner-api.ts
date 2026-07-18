export const BannerApi = {
  banners: '/banners',
  details: (id: string) => `/banners/${id}`,
  image: (id: string) => `/banners/${id}/image`
};
