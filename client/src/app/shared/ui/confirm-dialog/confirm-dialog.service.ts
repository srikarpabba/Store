import { inject, Service } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { map, Observable } from 'rxjs';
import { ConfirmDialog, ConfirmDialogData } from './confirm-dialog';

/**
 * Opens a confirmation dialog and emits exactly once:
 * true when confirmed, false when cancelled or dismissed.
 */
@Service()
export class ConfirmDialogService {

    private readonly dialog = inject(MatDialog);

    confirm(data: ConfirmDialogData): Observable<boolean> {
        return this.dialog
            .open(ConfirmDialog, { data, width: '22.5rem' })
            .afterClosed()
            .pipe(map(result => result === true));
    }
}
