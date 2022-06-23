import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PhoneFormService {

  constructor(
    private http: HttpClient,
  ) { }
  getAllCountries(): Observable<ICountry[]> {
    return this.http.get<ICountry[]>(`/assets/json/data.json`)
  }

}
export interface ICountry {
  code: string, name: { ar: string, en: string }, dialCode: number, flagEmoji: any
}