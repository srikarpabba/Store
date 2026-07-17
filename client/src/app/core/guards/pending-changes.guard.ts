import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { ConfirmDialogService } from '../../shared/ui/confirm-dialog/confirm-dialog.service';

/** Implemented by components the pendingChangesGuard protects */
export interface HasPendingChanges {
    hasPendingChanges(): boolean;
}

/**
 * Blocks in-app navigation away from a component with unsaved changes
 * until the user confirms. Browser refresh/close is not covered — that
 * would need a beforeunload listener.
 */
export const pendingChangesGuard: CanDeactivateFn<HasPendingChanges> = (component) => {

    if (!component.hasPendingChanges()) {
        return true;
    }

    return inject(ConfirmDialogService).confirm({
        title: 'Discard unsaved changes?',
        message: 'You have unsaved changes that will be lost if you leave this page.',
        confirmLabel: 'Discard',
        cancelLabel: 'Stay',
        destructive: true
    });
};
