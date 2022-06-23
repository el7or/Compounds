import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './shared/shared.module';

import { LocalStorageModule } from 'angular-2-local-storage';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { WebApiService } from './services/webApi.service';
import { BidiModule } from '@angular/cdk/bidi';
import { LanguageService } from './services/language.serives';
import { environment } from 'src/environments/environment';

export function httpTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    SharedModule,
    HttpClientModule,
    BidiModule,
    TranslateModule.forRoot({
      defaultLanguage: environment.defaultLanguage,
      loader: {
        provide: TranslateLoader,
        useFactory: httpTranslateLoader,
        deps: [HttpClient]
      }
    }),
    LocalStorageModule.forRoot({
      prefix: 'circle360-admin',
      storageType: 'localStorage'
    })
  ],
  providers: [WebApiService, LanguageService],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private languageService: LanguageService) {
    this.languageService.selectLang(this.languageService.lang, true);
  }
}
