import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OwnerUnitsService {

  private controller: string = 'OwnerUnits';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }


  // { ownerId: string, companyId: string}
  getOwnerUnits(params: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`, params);
  }

  postOwnerUnits(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
}
