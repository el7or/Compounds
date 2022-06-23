import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OwnerRegistrationsService {

  private controller: string = 'OwnerRegistrations';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  // /api/OwnerRegistrations/owners-with-filter
  // { Name: string, Phone: string, UserType: string, UserConfirmed: boolean, Companies: string[], Compounds: string[] }
  getFilteredOwnerRegistrations(params: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/owners-with-filter`, params);
  }
  // /api/OwnerRegistrations/owner
  // phone ownerRegistrationId companyId
  getOwnerRegistrations(params: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/owner`, params);
  }
}
