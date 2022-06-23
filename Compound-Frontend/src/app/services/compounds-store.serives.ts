import { Injectable } from '@angular/core';
import { LocalStorageService } from 'angular-2-local-storage';
import { ICompound } from './api/admin/compounds.service';

@Injectable({
    providedIn: 'root'
})
export class CompoundsStoreService {
    constructor(private localStorageService: LocalStorageService) { }

    get compounds(): ICompound[] {
        return this.localStorageService.get('~Compound~') as ICompound[];
    };
    set compounds(compound: ICompound[]) {
        this.localStorageService.set('~Compound~', compound);
    }
    clearCompounds() {
        this.localStorageService.remove('~Compound~');
    }
}