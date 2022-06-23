import { Injectable } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';

@Injectable({
    providedIn: 'root'
})
export class NotificationDrawerService {
    public badge: number | undefined;
    private drawer: MatDrawer | undefined;
    set drawerEl(el: MatDrawer) {
        this.drawer = el;
    }

    async open() {
        return await this.drawer?.open();
    }

    async close() {
        return await this.drawer?.close();
    }
}
