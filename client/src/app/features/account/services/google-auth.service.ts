import { inject, NgZone, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';

type GoogleButtonText = 'signin_with' | 'signup_with';

/**
 * Thin wrapper around Google Identity Services (GIS).
 * Loads the gsi/client script on demand and renders the official button.
 * https://developers.google.com/identity/gsi/web
 */
@Service()
export class GoogleAuthService {

    private readonly zone = inject(NgZone);

    private scriptLoaded: Promise<void> | null = null;

    /**
     * Renders the official Google button inside `container`.
     * `onCredential` receives the Google ID token to exchange with our API.
     */
    async renderButton(
        container: HTMLElement,
        text: GoogleButtonText,
        onCredential: (idToken: string) => void
    ): Promise<void> {

        await this.loadScript();

        const google = (window as any).google;

        google.accounts.id.initialize({
            client_id: environment.googleClientId,
            callback: (response: { credential: string }) =>
                this.zone.run(() => onCredential(response.credential))
        });

        google.accounts.id.renderButton(container, {
            type: 'standard',
            theme: 'outline',
            size: 'large',
            shape: 'rectangular',
            text,
            logo_alignment: 'left',
            width: 352
        });
    }

    private loadScript(): Promise<void> {
        this.scriptLoaded ??= new Promise<void>((resolve, reject) => {
            const script = document.createElement('script');
            script.src = 'https://accounts.google.com/gsi/client';
            script.async = true;
            script.defer = true;
            script.onload = () => resolve();
            script.onerror = () => {
                this.scriptLoaded = null;
                reject(new Error('Failed to load Google Identity Services'));
            };
            document.head.appendChild(script);
        });

        return this.scriptLoaded;
    }
}
