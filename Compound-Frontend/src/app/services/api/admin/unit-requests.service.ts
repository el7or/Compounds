import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UnitRequestsService {

  private controller: string = 'UnitRequests';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }


  // /api/UnitRequests/{requestId}/approve
  getApprove(requestId: any) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${requestId}/approve`);
  }
  // /api/UnitRequests/approve
  postApprove(body: { compoundOwnerId: string, ownerRegistrationId: string }) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}/approve`, body);
  }
}
