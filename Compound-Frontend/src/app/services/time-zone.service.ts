import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimeZoneService {

  constructor(
    private http: HttpClient,
  ) { }

  getAllTimeZones(): Observable<ITimeZone[]> {
    return this.http.get<ITimeZone[]>(`/assets/json/timezones.json`)
  }
}

export interface ITimeZone {
  value: string,
  offset: string,
  text: string,
}
