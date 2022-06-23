import { LanguageService } from './language.serives';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpParams, HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CurrentUserService } from './current-user.serives';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class WebApiService {
  constructor(
    private http: HttpClient,
    private currentUserService: CurrentUserService,
    private languageService: LanguageService,
    private router: Router
  ) { }

  private setHeaders(file = false): HttpHeaders {
    let headersConfig;
    if (file) {
      headersConfig = {}
    } else {
      headersConfig = {
        'content-type': 'application/json;charset=utf-8',
        'accept': 'application/json',
        'language': this.languageService.lang,
        'timezone': ((new Date().getTimezoneOffset() / 60) * -1).toString(),
        'authorization': `Bearer ${String(this.currentUserService.currentUser?.accessToken || '')}`
      };
    }
    return new HttpHeaders(headersConfig);
  }
  private logError(controller: string, error: any) {
    if (error.status === 401)
      this.router.navigate(['auth', 'login']);
    else console.log(controller, error);
  }
  private log(controller: string, res: any) {
    if (!environment.production)
      console.log(controller, res);
  }
  private toQueryString(obj: { [x: string]: any; }) {
    var parts = [];
    for (var property in obj) {
      var value = obj[property]
      if (value != null && value != undefined)
        parts.push(encodeURIComponent(property) + '=' + encodeURIComponent(value))
    }
    return parts.join('&');
  }
  get(serviceSubDomain: string, controller: string, params?: HttpParams): Observable<ResultViewModel> {
    return this.http.get<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}`,
      { headers: this.setHeaders(), params }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
  getWithFilter(serviceSubDomain: string, controller: string, filterObject: Object = {}): Observable<ResultViewModel> {
    return this.http.get<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}?${this.toQueryString(filterObject)}`,
      { headers: this.setHeaders() }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
  post(serviceSubDomain: string, controller: string, body: Object = {}): Observable<ResultViewModel> {
    return this.http.post<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}`,
      body, { headers: this.setHeaders() }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
  put(serviceSubDomain: string, controller: string, body: Object = {}): Observable<ResultViewModel> {
    return this.http.put<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}`,
      body, { headers: this.setHeaders() }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
  delete(serviceSubDomain: string, controller: string, params?: HttpParams): Observable<ResultViewModel> {
    return this.http.delete<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}`,
      { headers: this.setHeaders(), params }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
  postFile(serviceSubDomain: string, controller: string, body: Object = {}): Observable<ResultViewModel> {
    return this.http.post<ResultViewModel>(`${environment.protocol}://${serviceSubDomain}${environment.appUrl}${controller}`,
      body, { headers: this.setHeaders() }).pipe(tap(res => this.log(controller, res), error => this.logError(controller, error)))
  }
}

export interface ResultViewModel {
  result: any;
  ok: boolean;
  message: string;
  statusCode: number;
}

export class PagedInput {
  pageNumber: number = 1;
  pageSize: number = 20;
  sortBy: string = 'name';
  isSortAscending: boolean = true;
  public constructor(init?: Partial<PagedInput>) {
    Object.assign(this, init);
  }
}

export class Iimage {
  sizeInBytes?: number;
  fileName?: string;
  path?: string;
  fileBase64?: string;
}
