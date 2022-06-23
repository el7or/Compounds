import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { WebApiService } from '../../webApi.service';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SubOwnersService {

  private controller: string = 'OwnerSubUsers';
  private serviceSubDomain: string = environment.serviceSubDomain_owners;

  constructor(private webApi: WebApiService) { }

  getSubOwners(params: HttpParams) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}`, params);
  }
  getSubOwnerById(ownerRegisterationId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${ownerRegisterationId}`);
  }
  postSubOwner(body: any) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putSubOwner(body: any) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  DeleteSubOwner(ownerRegisterationId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${ownerRegisterationId}`);
  }
  activeSubOwner(ownerRegisterationId: string) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${ownerRegisterationId}/active`);
  }
  deActiveSubOwner(ownerRegisterationId: string) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}/${ownerRegisterationId}/disactive`);
  }
}

export class SubOwner {
  ownerRegistrationId!: string;
  name!: string;
  phone!: string;
  email!: string;
  gender!: string;
  address!: string;
  whatsAppNum!: string;
  birthDate!: string;
  image!: string;
  registerDate!:Date;
  userType!: any;
  units!:SubOwnerUnit[];
  isActive!:boolean;
}

export class SubOwnerUnit{
  compound_Unit_Id!:string;
  compoundUnitName!:string;
  start_From!:Date;
  end_To!:Date;
}

export enum OwnerType {
  Owner = 1,
  SubUser = 2,
  Tenant = 3
}
