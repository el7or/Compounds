import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompaniesService {

  private controller: string = 'Companies';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getCompanies() {
    return this.webApi.get(this.serviceSubDomain,`${this.controller}`);
  }
  postCompanies(company: any) {
    return this.webApi.post(this.serviceSubDomain,`${this.controller}`, company);
  }
  putCompanies(company: any) {
    return this.webApi.put(this.serviceSubDomain,`${this.controller}/${company.id}`, company);
  }
  DeleteCompanies(companyId: any) {
    return this.webApi.put(this.serviceSubDomain,`${this.controller}/${companyId}`);
  }

  getCompanyCompounds(companyId: any) {
    return this.webApi.get(this.serviceSubDomain,`${this.controller}/compounds/${companyId}`);
  }
}

export interface ICompany {
  logo: String;
  name_Ar: String;
  name_En: String;
  phone: String;
  whatsAppNum: String;
  website: String;
  address: String;
  location: {
    latitude: number;
    longitude: number;
  };

  planId: String;

  email: String;
  password: String;
}
