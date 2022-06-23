import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LocalStorageService } from 'angular-2-local-storage';
import { environment } from 'src/environments/environment';
type Languages = 'en' | 'ar';

@Injectable({
    providedIn: 'root'
})
export class LanguageService {
    private currentLang: Languages = this.localStorageService.get('current~lang') || environment.defaultLanguage;

    constructor(
        @Inject(DOCUMENT) private document: Document,
        // dir: Directionality,
        private translateService: TranslateService,
        private localStorageService: LocalStorageService
    ) { }

    get lang(): Languages {
        return this.currentLang;
    }

    selectLang(lang: Languages, dontRelaod: boolean = false) {
        if (lang === 'ar') {
            this.document.body.dir = 'rtl';
        } else if (lang === 'en') {
            this.document.body.dir = 'ltr';
        }
        this.document.body.lang = lang;

        this.translateService.use(lang);
        this.localStorageService.set('current~lang', lang);
        if (!dontRelaod)
            this.document.location.reload();
    }
    getName(obj: any): string {
        let name: string = '';
        if (this.currentLang === 'en') {
            name = obj['nameEn']
        } else if (this.currentLang === 'ar') {
            name = obj['nameAr']
        }
        if (name)
            return name;
        else
            return obj['name'];
    }
}
