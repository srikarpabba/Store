import { inject, Service } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Service()
export class NotificationService {

    private readonly snackBar = inject(MatSnackBar);

    private static readonly DURATION_MS = 4000;

    success(message: string): void {
        this.open(message, 'snackbar--success');
    }

    error(message: string): void {
        this.open(message, 'snackbar--error');
    }

    info(message: string): void {
        this.open(message);
    }

    private open(message: string, panelClass?: string): void {
        this.snackBar.open(message, 'Dismiss', {
            duration: NotificationService.DURATION_MS,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass
        });
    }
}
