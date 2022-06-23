import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { WebApiService } from '../../webApi.service';
import { ServiceSubType } from './compound-services.service';

@Injectable({
  providedIn: 'root'
})
export class ServiceSubTypesService {

  private controller: string = 'ServiceSubTypes';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getSubTypesByTypeId(compoundId: string, serviceTypeId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${compoundId}/${serviceTypeId}`);
  }
  postServiceSubType(body: ServiceSubType) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putServiceSubType(body: ServiceSubType) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  deleteServiceSubType(serviceSubTypeId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${serviceSubTypeId}`);
  }
}
