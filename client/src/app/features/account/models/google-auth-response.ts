import { AccessTokensResponse } from './access-tokens-response';

export interface GoogleAuthResponse extends AccessTokensResponse {
    /** True when this Google sign-in just created the account */
    isNewUser: boolean;
}
