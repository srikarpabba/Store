export interface Profile {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string | null;
    emailConfirmed: boolean;
    hasPassword: boolean;
}
