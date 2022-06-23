import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class LoadingService {
    private isItLoading$: Subject<boolean> = new Subject<boolean>();

    get isItLoading(): Observable<boolean> {
        return this.isItLoading$.asObservable();
    }

    set itIsLoading(isItLoading: boolean) {
        this.isItLoading$.next(isItLoading);
    }
}