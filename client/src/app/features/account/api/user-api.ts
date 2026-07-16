export const UserApi = {
  me: '/users/me',
  resendConfirmation: '/users/me/resend-confirmation',
  addresses: '/users/me/addresses',
  address: (id: string) => `/users/me/addresses/${id}`,
  defaultAddress: (id: string) => `/users/me/addresses/${id}/default`
};
