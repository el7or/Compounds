import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PlansService {

  private controller: string = 'Plans';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getPlans() {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`);
  }
  postPlans(company: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, company);
  }
  putPlans(company: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${company.id}`, company);
  }
  DeletePlans(companyId: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${companyId}`);
  }
}
