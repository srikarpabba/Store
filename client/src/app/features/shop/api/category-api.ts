export const CategoryApi = {
  categories: '/categories',
  details: (id: string) => `/categories/${id}`,
  photo: (categoryId: string, genderId: string) => `/categories/${categoryId}/genders/${genderId}/photo`
};
