import { ComponentPortal, ComponentType, Portal } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { from, Observable, Subject } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class RightDrawer {
    panel: MatSidenav | undefined;
    private panelPortal$: Subject<Portal<any>> = new Subject<Portal<any>>();
    private parentData$: any | undefined;
    private parentReturn$: Subject<any> = new Subject();

    get panelPortal(): Observable<Portal<any>> {
        return this.panelPortal$.asObservable();
    }

    get parentData(): any {
        return this.parentData$;
    }

    open(component: ComponentType<any>, data: any = null) {
        this.parentData$ = data;
        let portal: Portal<any> = new ComponentPortal(component);
        this.panelPortal$.next(portal);
        this.panel?.open();
        this.panel?.closedStart.subscribe(() => this.close());
        return this.parentReturn$.pipe(take(1));
    }

    close(data: any = null) {
        if (data) this.parentData$ = undefined;
        this.parentReturn$.next(data);
        return this.panel?.close();
    }
}
